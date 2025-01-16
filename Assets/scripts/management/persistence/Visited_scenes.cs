using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;

namespace rvinowise.unity {
public class Visited_scenes : MonoBehaviour
{
   private void Start() {
      Scene current_scene = SceneManager.GetActiveScene();
      Save_load_game.instance.save_progress();
   }

   public void save_progress() {
      Newtonsoft.Json.JsonSerializer json = new JsonSerializer();
      FileStream file = File.Create(Application.persistentDataPath + "/game_progress.dat");
    
      Player_progress data = new Player_progress();
      //data.visited_scenes =;
      data.score = 666;
    
      //json.Serialize(file, data);
      file.Close();
   }
   
   public void load_progress() {
      
   }
   
  

}

}