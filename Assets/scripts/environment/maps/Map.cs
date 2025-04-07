


using System;
using UnityEngine;
using rvinowise.contracts;

namespace rvinowise.unity {


    
public class Map: MonoBehaviour {

    public UnityEngine.Rect rect;
    public float ground_z;

    public static Map instance;
    public Save_load_game save_load_game;

    public static float max_complexity = 30f; //to prevent laggs by limiting the amount of units
    public float current_complexity;
    
    private void Awake() {
        Contract.Requires(instance == null, "Map is a singleton");
        instance = this;
    }

    private void Start() {
        save_load_game.save_at_checkpoint();
    }

    public bool is_complexity_exceeded() {
        return current_complexity >= max_complexity;
    }
    
    public bool has(
        Transform other_transform
    ) {
        if (rect.Contains(other_transform.position)) {
            return true;
        }
        return false;
    }


}

}