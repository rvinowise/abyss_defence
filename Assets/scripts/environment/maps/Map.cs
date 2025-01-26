


using System;
using UnityEngine;
using rvinowise.contracts;

namespace rvinowise.unity {


    
public class Map: MonoBehaviour {

    public UnityEngine.Rect rect;
    public float ground_z;

    public static Map instance;
    public Save_load_game save_load_game;

    private void Awake() {
        Contract.Requires(instance == null, "Map is a singleton");
        instance = this;
    }

    private void Start() {
        save_load_game.save_at_checkpoint();
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