using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Assertions;


namespace rvinowise.unity {
public class Save_load_game : MonoBehaviour {

   public static Save_load_game instance = null;
   private void Awake() {
      Debug.Assert(instance == null, "singleton");
      instance = this;
   }

   public void save_progress() {
      
   }
   
   public void load_progress() {
      
   }
   
  
   public void save_to_binary()
   {
      BinaryFormatter binary_formatter = new BinaryFormatter();
      FileStream file = File.Create(Application.persistentDataPath + "/playerInfo.dat");
    
      Player_progress data = new Player_progress();
      //data.visited_scenes =;
      data.score = 666;
    
      binary_formatter.Serialize(file, data);
      file.Close();
   }
  
   public void load_from_binary()
   {
      if(File.Exists(Application.persistentDataPath + "/playerInfo.dat"))
      {
         BinaryFormatter binary_formatter = new BinaryFormatter();
         FileStream file = File.Open(Application.persistentDataPath + "/playerInfo.dat", FileMode.Open);
         Player_progress data = (Player_progress)binary_formatter.Deserialize(file);
         file.Close();
      
         
      }
   }
   
}
}

