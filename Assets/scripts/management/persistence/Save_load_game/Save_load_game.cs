using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using rvinowise.unity;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu]
public class Save_load_game : 
    ScriptableObject
{
    public void save_at_checkpoint() {
        Scene current_scene = SceneManager.GetActiveScene();

        var file_unique_id = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        var save_filename = $"{Application.persistentDataPath}/saved_checkpoint_{file_unique_id}.dat";
    
        Saved_game saved_data = new Saved_game();
        saved_data.scene = current_scene.name;
        saved_data.score = 666;
    
        var saved_data_string = JsonConvert.SerializeObject(saved_data, Formatting.Indented);
        File.WriteAllText(save_filename,saved_data_string);
    }

    public IList<Saved_game> load_all_checkpoints_from_files() {
        var result = new List<Saved_game>();
        foreach (string file in Directory.EnumerateFiles(Application.persistentDataPath, "*.dat")) {
            string loaded_data_string = File.ReadAllText(file);
            var loaded_data = JsonConvert.DeserializeObject<Saved_game>(loaded_data_string);
            result.Add(loaded_data);
        }
        return result;
    }
   
    public void load_at_checkpoint() {
      
    }
}
