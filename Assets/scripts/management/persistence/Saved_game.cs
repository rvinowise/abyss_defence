using System;
using Newtonsoft.Json;


namespace rvinowise.unity {

public struct Saved_game {
    public String scene;
    public int score;
    public DateTime saved_at;

    [JsonIgnore]
    public string file_path;
}

}