using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;
using UnityEngine.Events;
using Newtonsoft.Json;
using System;
using System.Threading;

public class WebSocketClientManager 
{
    public static WebSocket webSocket;
    public static UnityAction<Dictionary<string, PlayerActionData>> recieveCompletedHandler;
   

   public static void Connect()
   {
      if (webSocket == null)
      {
        webSocket = new WebSocket("ws://52.195.216.216:3000");
        webSocket.OnMessage += (sender, e) => RecieveAllUserAction(e.Data);
        webSocket.Connect();
      }
   }

   public static void DisConnect()
   {
     webSocket.Close();
     webSocket = null;
   }
   
   public static void SendPlayerAction(string action, Vector3 pos, string way, float range, Vector3 rotation)
   {
      var userActionData = new PlayerActionData
      {
          action = action,
          way = way,
          room_no = 1,
          user = UserLoginName.userName,
          pos_x = pos.x,
          pos_y = pos.y,
          pos_z = pos.z,
          range = range,
          rotation_x = rotation.x,
          rotation_y = rotation.y,
          rotation_z = rotation.z,
      };

      webSocket.Send(userActionData.ToJson());
   }


   public static void RecieveAllUserAction(string json)
   {
      var allUserActionHash = PlayerActionData.FromJson(json, 1);
      recieveCompletedHandler?.Invoke(allUserActionHash);
   }


}
