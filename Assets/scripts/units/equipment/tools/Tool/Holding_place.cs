using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise.unity.extensions;

using rvinowise;
using rvinowise.unity.geometry2d;
using rvinowise.contracts;
using rvinowise.unity.units.parts.limbs.arms;
using UnityEngine.Serialization;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;

namespace rvinowise.unity.units.parts.tools {

public class Holding_place: MonoBehaviour
{
    //public Vector2 place_on_tool = Vector2.zero;
    //public Degree grip_direction = new Degree(0);
    [SerializeField]
    public bool is_main;
    [SerializeField] 
    public string grip_gesture_from_editor = ""; 
    
    public Hand_gesture grip_gesture = Hand_gesture.Relaxed;
    
    public Quaternion grip_direction_quaternion {
        get {
            return transform.localRotation;
            //return grip_direction.to_quaternion();
        }
    }
    public Vector2 place_on_tool {
        set { transform.localPosition = value; }
        get { return transform.localPosition; }
    }
    public Degree grip_direction {
        set { transform.localRotation = value.to_quaternion(); }
        get { return transform.localRotation.to_degrees(); }
    }

    public Tool tool { get; set; }
    
    public Hand holding_hand {
        get { return _holding_hand; }
        private set {
            _holding_hand = value;
        }
    }
    public Hand _holding_hand;

    public IHave_velocity holder {
        get { return holding_hand; }
    }

    public Vector2 position {
        get{
            //return tool.transform.TransformPoint(place_on_tool);
            return transform.position;
        }
    }
    public Quaternion rotation {
        get {
            //return tool.transform.rotation * grip_direction_quaternion;
            return transform.rotation;
        }
    }
    

    public static Holding_place main(Tool in_tool) {
        Holding_place holding_place = Holding_place.create(in_tool.transform);
        holding_place.is_main = true;
        holding_place.grip_gesture = Hand_gesture.Grip_of_vertical;

        return holding_place;
    }
    public static Holding_place secondary(Tool in_tool) {
        Holding_place holding_place = Holding_place.create(in_tool.transform);
        holding_place.is_main = false;
        holding_place.grip_gesture = Hand_gesture.Grip_of_vertical;

        return holding_place;
    }

    public static Holding_place create(Transform in_parent) {
        Tool parent_tool =  in_parent.GetComponent<Tool>();
        Contract.Requires(parent_tool  != null, "the parent of a Holding_place must be a Tool");
        
        GameObject game_object = new GameObject();
        game_object.transform.parent = in_parent;
        Holding_place holding_place = game_object.add_component<Holding_place>();
        holding_place.tool = parent_tool;
        
        return holding_place;
    }
    public static Holding_place create(
        bool _is_main,
        Hand_gesture _hand_gesture,// = Hand_gesture.Relaxed,
        Vector2 _position,// = Vector2.zero,
        Quaternion _rotation// = Quaternion.identity
    ) {
        GameObject game_object = new GameObject();
        Holding_place holding_place = game_object.add_component<Holding_place>();
        holding_place.is_main = _is_main;
        holding_place.grip_gesture = _hand_gesture;
        holding_place.transform.localPosition = _position;
        holding_place.transform.localRotation = _rotation;

        return holding_place;
    }
    


    protected void Awake() {
        //base.Awake();
        Tool parent_tool =  transform?.parent.GetComponent<Tool>();
        Contract.Requires(parent_tool  != null, "the parent of a Holding_place must be a Tool");

        tool = parent_tool;
        if (grip_gesture_from_editor != "") {
            grip_gesture = Hand_gesture.Parse(grip_gesture_from_editor);
        }
    }
    
    public void hold_by(Hand in_hand) {
        holding_hand = in_hand;
        if (is_main) {
            tool.transform.parent = in_hand?.valuable_point.transform;

            Vector2 inversed_position = -this.transform.localPosition;
            Quaternion inversed_rotation = this.transform.localRotation.inverse();
            
            tool.transform.localPosition =
                inversed_position.rotate(inversed_rotation);

            tool.transform.localRotation =
                inversed_rotation;
        }
    }
    
}
}