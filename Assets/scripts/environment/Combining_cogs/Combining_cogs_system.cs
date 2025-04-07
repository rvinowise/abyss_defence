using System;
using System.Collections;
using System.Collections.Generic;
using rvinowise.unity.actions;
using rvinowise.unity.geometry2d;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;


namespace rvinowise.unity {


    
public class Combining_cogs_system : MonoBehaviour {

    [Serializable]
    public struct Cog_and_rotation {
        public Turning_element_actor cog;
        public float rotation;
    }
    
    public Turning_element_actor cog1;
    public Turning_element_actor cog2;
    public Team team;

    public List<Cog_and_rotation> cogs_with_rotation;
    //public List<Tuple<Turning_element_actor, float>> turning_actors_with_rotation;

    public Action_runner action_runner;
    
    void Awake() {
        // cogs_with_rotation = new List<Tuple<Turning_element_actor, float>> {
        //     cog1,cog2
        // };
    }
    

    public void invoke_next_creature() {

        Combining_cogs_invoke_creature.create(
            this
        ).set_on_completed(on_rings_reached_direction)
        .start_as_root(action_runner);
    }
    
    [ContextMenu("create_next_combination")]
    void create_next_combination() {
        invoke_next_creature();
    }
    
    private void FixedUpdate() {
        action_runner.update();
    }

    public void on_rings_reached_direction() {
    }


    
}

    
}