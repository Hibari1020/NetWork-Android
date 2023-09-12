using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class PlayerActionData 
{
   [JsonProperty("action")]
   public string action;

   [JsonProperty("room_no")]
   public int? room_no;

   [JsonProperty("user")]
   public string user;

   [JsonProperty("pos_x")]
   public float pos_x;

   [JsonProperty("pos_y")]
   public float pos_y;

   [JsonProperty("pos_z")]
   public float pos_z;

   [JsonProperty("way")]
   public string way;

   [JsonProperty("range")]
   public float range;

   [JsonProperty("rotation_x")]
   public float rotation_x;

   [JsonProperty("rotation_y")]
   public float rotation_y;

   [JsonProperty("rotation_z")]
   public float rotation_z;


   public string ToJson()
   {
      return JsonConvert.SerializeObject(this, Formatting.None);
   }

   public static Dictionary<string, PlayerActionData> FromJson(string json, int roomNo)
   {
      var jsonHash = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, Dictionary<string, object>>>>(json);

      var playerActionHash = new Dictionary<string, PlayerActionData>();

      if(!jsonHash.ContainsKey("room" + roomNo))
      {
        return playerActionHash;
      }

      var roomPlayerHash = jsonHash["room" + roomNo];
      foreach (var playerHash in roomPlayerHash)
      {
         var PlayerActionData = new PlayerActionData
         {
            action = (string)playerHash.Value["action"],
            user = (string)playerHash.Value["user"],
            pos_x = float.Parse(playerHash.Value["pos_x"].ToString()),
            pos_y = float.Parse(playerHash.Value["pos_y"].ToString()),
            pos_z = float.Parse(playerHash.Value["pos_z"].ToString()),
            way =(string)playerHash.Value["way"],
            range = float.Parse(playerHash.Value["range"].ToString()),
            rotation_x = float.Parse(playerHash.Value["rotation_x"].ToString()),
            rotation_y = float.Parse(playerHash.Value["rotation_y"].ToString()),
            rotation_z = float.Parse(playerHash.Value["rotation_z"].ToString()),
         };
         playerActionHash.Add(PlayerActionData.user, PlayerActionData);
      }

      return playerActionHash;
   }
}
