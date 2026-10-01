using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using rvinowise.unity;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu]
public class Save_load_game : 
    ScriptableObject
{
    public const string last_scene_prefs_key = "last_checkpoint_scene";

    public void save_at_checkpoint() {
        Scene current_scene = SceneManager.GetActiveScene();

        var file_unique_id = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        var save_filename = $"{Application.persistentDataPath}/saved_checkpoint_{file_unique_id}.dat";
    
        Saved_game saved_data = new Saved_game();
        saved_data.scene = current_scene.name;
        saved_data.score = 0;
        saved_data.saved_at = DateTime.Now;
    
        var saved_data_string = JsonConvert.SerializeObject(saved_data, Formatting.Indented);
        File.WriteAllText(save_filename,saved_data_string);
        PlayerPrefs.SetString(last_scene_prefs_key, current_scene.name);
        PlayerPrefs.Save();
    }

    public IList<Saved_game> load_all_checkpoints_from_files() {
        var result = new List<Saved_game>();
        if (!Directory.Exists(Application.persistentDataPath)) {
            return result;
        }
        foreach (string file in Directory.EnumerateFiles(Application.persistentDataPath, "*.dat")) {
            string loaded_data_string = File.ReadAllText(file);
            var loaded_data = JsonConvert.DeserializeObject<Saved_game>(loaded_data_string);
            loaded_data.file_path = file;
            if (loaded_data.saved_at == default) {
                loaded_data.saved_at = File.GetLastWriteTime(file);
            }
            result.Add(loaded_data);
        }
        return result.OrderByDescending(saving => saving.saved_at).ToList();
    }

    public Saved_game? latest_checkpoint() {
        var savings = load_all_checkpoints_from_files();
        if (savings.Count > 0) {
            return savings[0];
        }
        var last_scene = PlayerPrefs.GetString(last_scene_prefs_key, "");
        if (!string.IsNullOrEmpty(last_scene)) {
            return new Saved_game {
                scene = last_scene,
                saved_at = DateTime.Now
            };
        }
        return null;
    }
   
    public void load_at_checkpoint(Saved_game saved) {
        if (string.IsNullOrEmpty(saved.scene)) {
            Debug.LogError("checkpoint has no scene name");
            return;
        }
        if (!Application.CanStreamedLevelBeLoaded(saved.scene)) {
            Debug.LogError($"checkpoint scene '{saved.scene}' is not in the build");
            return;
        }
        SceneManager.LoadScene(saved.scene);
    }
}
