using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise;
using rvinowise.units;


namespace rvinowise {

public class Game_object:Child {

    public readonly GameObject game_object; 
    
    
    /* GameObject fields */
    public Quaternion rotation {
        get {
            return transform.rotation;
        }
        set => transform.rotation = value;
    }
    public Transform transform {
        get {
            return game_object.transform;
        }
    }
    
    public Vector2 position {
        get {
            return transform.position;
        }
        set {
            transform.position = value;
        }
    }
    
    public Vector2 direction {
        get {
            return transform.right;
        }
        set {
            float needed_angle = Mathf.Atan2(value.y, value.x) * Mathf.Rad2Deg;
            transform.eulerAngles = Vector3.forward * needed_angle;
        }
    }
    
    public SpriteRenderer spriteRenderer {
        get {
            return game_object.GetComponent<SpriteRenderer>();
        }
    }
    
    /*  Game_object itself */

    public Vector2 local_position { //same as attachment but with Unity's parenting
        get { return game_object.transform.localPosition; }
        set { game_object.transform.localPosition = value; }
    }
    
    public Game_object() {
        game_object = new GameObject();
        game_object.AddComponent<SpriteRenderer>();
    }
    public Game_object(string name) {
        game_object = new GameObject(name);
        game_object.AddComponent<SpriteRenderer>();
    }
    public void direct_to(Vector2 in_aim) {
        transform.direct_to(in_aim);
    }
    public void set_direction(float in_direction) {
        transform.set_direction(in_direction);
    }

    public void attach_to_host() {
        position = host.TransformPoint(attachment);
    }

    public void update() {
        attach_to_host();
    }

}
}