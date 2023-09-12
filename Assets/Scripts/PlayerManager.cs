using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;
using System;

public class PlayerManager : MonoBehaviour
{
    private const float KEY_MOVEMENT = 0.5f;
    private GameObject playerPrefab = null;
    private GameObject player;
    IDisposable updateDisposable;
    float speed = 3f;
    Animator _animator;
    Vector3 dir;

    private Dictionary<string, PlayerActionData> PlayerActionMap;

    private readonly Dictionary<string, GameObject> playerObjectMap = new Dictionary<string, GameObject>();


    // Start is called before the first frame update
    void Start()
    {
        player = MakePlayer(Vector3.zero, UserLoginName.userName);
        _animator = player.GetComponent<Animator>();
        StartWebSocket();

        this.UpdateAsObservable()
            .Where(_ => PlayerActionMap != null)
            .Subscribe(_ => 
            {
                Synchronize();
                PlayerActionMap = null;
            })
            .AddTo(this);

    }

    private void StartWebSocket()
    {
        WebSocketClientManager.Connect();
        WebSocketClientManager.recieveCompletedHandler += OnRecieveMessage;
        WebSocketClientManager.SendPlayerAction("connect", Vector3.zero, "neutral", 0.0f, Vector3.zero);
    }

    public void UpMove()
    {
        _animator.SetBool("Running", true);
        updateDisposable = this.UpdateAsObservable()
               .Subscribe(_ => 
               {
                    Vector3 moveDistance = new Vector3(0, 0, -1 * KEY_MOVEMENT);
                    var targetPos = player.transform.position + moveDistance;
                    dir = (targetPos - player.transform.position).normalized;
                    player.transform.rotation = Quaternion.LookRotation(dir);
                    player.transform.position += dir * speed * Time.deltaTime;
                    WebSocketClientManager.SendPlayerAction("move", player.transform.position, "up", KEY_MOVEMENT, dir);
               });
    }

    public void DownMove()
    {
         _animator.SetBool("Running", true);
         updateDisposable = this.UpdateAsObservable()
                .Subscribe(_ => 
                {
                    Vector3 moveDistance = new Vector3(0, 0, KEY_MOVEMENT);
                    var targetPos = player.transform.position + moveDistance;
                    dir = (targetPos - player.transform.position).normalized;
                    player.transform.rotation = Quaternion.LookRotation(dir);
                    player.transform.position += dir * speed * Time.deltaTime;
                    WebSocketClientManager.SendPlayerAction("move", player.transform.position, "down", KEY_MOVEMENT, dir);
                });
    }

    public void LeftMove()
    {
         _animator.SetBool("Running", true);
         updateDisposable = this.UpdateAsObservable()
                .Subscribe(_ => 
                {
                    Vector3 moveDistance = new Vector3(KEY_MOVEMENT, 0, 0);
                    var targetPos = player.transform.position + moveDistance;
                    dir = (targetPos - player.transform.position).normalized;
                    player.transform.rotation = Quaternion.LookRotation(dir);
                    player.transform.position += dir * speed * Time.deltaTime;
                    WebSocketClientManager.SendPlayerAction("move", player.transform.position, "left", KEY_MOVEMENT, dir);
                });
    }

    public void RightMove()
    {
       _animator.SetBool("Running", true);
       updateDisposable = this.UpdateAsObservable()
                .Subscribe(_ => 
                {
                    Vector3 moveDistance = new Vector3(-1 * KEY_MOVEMENT, 0, 0);
                    var targetPos = player.transform.position + moveDistance;
                    dir = (targetPos - player.transform.position).normalized;
                    player.transform.rotation = Quaternion.LookRotation(dir);
                    player.transform.position += dir * speed * Time.deltaTime;
                    WebSocketClientManager.SendPlayerAction("move", player.transform.position, "right", KEY_MOVEMENT, dir);
                });
    }

    public void Stop()
    {
        _animator.SetBool("Running", false);
        updateDisposable.Dispose();
        WebSocketClientManager.SendPlayerAction("stop", player.transform.position, "neutral", KEY_MOVEMENT, dir);
    }

    private void OnRecieveMessage(Dictionary<string, PlayerActionData> PlayerActionMap)
    {
        this.PlayerActionMap = PlayerActionMap;
    }

    private void Synchronize()
    {
        List<string> otherPlayerNameList = new List<string>(playerObjectMap.Keys);
        foreach(var otherPlayerName in otherPlayerNameList)
        {
            if (!PlayerActionMap.ContainsKey(otherPlayerName))
            {
                Destroy(playerObjectMap[otherPlayerName]);
                playerObjectMap.Remove(otherPlayerName);
            }
        }

        foreach(var playerAction in PlayerActionMap.Values)
        {
            if (UserLoginName.userName == playerAction.user)
            {
                continue;
            }

            if(playerObjectMap.ContainsKey(playerAction.user))
            {
                playerObjectMap[playerAction.user].transform.position = GetMovePos(playerAction);
                playerObjectMap[playerAction.user].transform.rotation = Quaternion.LookRotation(GetRotation(playerAction));
                GetAction(playerAction, playerObjectMap[playerAction.user]);

            }
            else 
            {
                var player = MakePlayer(GetMovePos(playerAction), playerAction.user);
                playerObjectMap.Add(playerAction.user, player);
            }
        }
    }


    private GameObject MakePlayer(Vector3 pos, string name)
    {
        playerPrefab = playerPrefab??(GameObject)Resources.Load("Player");
        var player = (GameObject)Instantiate(playerPrefab, pos, Quaternion.identity);

        var otherNameText = player.transform.Find("UserName").gameObject;
        otherNameText.GetComponent<TextMesh>().text = name;

        return player;
    }

    private Vector3 GetMovePos(PlayerActionData playerAction)
    {
        var pos = new Vector3(playerAction.pos_x, playerAction.pos_y, playerAction.pos_z);
        pos.z += (playerAction.way == "up")?playerAction.range : 0;
        pos.z -= (playerAction.way == "down")?playerAction.range : 0;
        pos.x -= (playerAction.way == "left")?playerAction.range : 0;
        pos.x += (playerAction.way == "right")?playerAction.range : 0;

        return pos;
    }

    private Vector3 GetRotation(PlayerActionData playerAction)
    {
       var rotation = new Vector3(playerAction.rotation_x, playerAction.rotation_y, playerAction.rotation_z);
       
       return rotation;
    }

    private void GetAction(PlayerActionData playerAction, GameObject player)
    {
        var _animator = player.GetComponent<Animator>();
        if(playerAction.action == "stop")
        {
            _animator.SetBool("Running", false);
        }
        else if (playerAction.action == "move")
        {
            _animator.SetBool("Running", true);
        }
    }
}
