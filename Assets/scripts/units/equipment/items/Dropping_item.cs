using System;
using System.Collections.Generic;
using System.Linq;
using Animancer;
using rvinowise.unity.effects.trails.mesh_impl;
using rvinowise.unity.geometry2d;
using UnityEngine;
using rvinowise.unity.extensions;
using UnityEngine.Serialization;


namespace rvinowise.unity {



public class Dropping_item: 
    MonoBehaviour 
    ,IDestructible
{

    public struct Chance_of_dropping {
        public Transform item;
        public float chance;
    }
    
    public AnimancerComponent animancer;

    public List<Chance_of_dropping> dropped_items;
    
    void Awake() {
        
    }



    public void drop_item() {
    //    dropped_items
    }
    
    public void die() {
        drop_item();
    }


}


}