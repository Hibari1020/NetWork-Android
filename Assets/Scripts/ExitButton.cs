using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ExitButton : MonoBehaviour
{
    public Button _exitButton;

    // Start is called before the first frame update
    void Start()
    {
        _exitButton.onClick.AddListener(Exit);
    }

    private void Exit()
    {
        WebSocketClientManager.SendPlayerAction("disconnect", Vector3.zero, "neutral", 0.0f, Vector3.zero);
        WebSocketClientManager.DisConnect();
        SceneManager.LoadScene("TitleScene");
    }
}
