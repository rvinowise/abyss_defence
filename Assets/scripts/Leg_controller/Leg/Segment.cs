using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace units {
namespace limbs {

public class Segment {
    /* constant characteristics */
    public Vector2 tip {
        get {
            return _tip;
        }
        set {
            _tip = value;
            length = tip.magnitude;
            sqr_length = Mathf.Pow(length,2);
        }
    }
    private Vector2 _tip;
    public Span span;

    /* current characteristics */
    public Quaternion desired_relative_direction; //relative to attachment
    public GameObject game_object;

    public float length {
        get;
        private set;
    }
    public float sqr_length {
        get;
        private set;
    }

    public Segment(string name) {
        game_object = new GameObject(name);
        game_object.AddComponent<SpriteRenderer>();
    }
    public SpriteRenderer spriteRenderer {
        get {
            return game_object.GetComponent<SpriteRenderer>();
        }
    }

    /* current characteristics */
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
            //transform.rotation = Quaternion.LookRotation(value);
            float needed_angle = Mathf.Atan2(value.y, value.x) * Mathf.Rad2Deg;
            //float diff_angle = Mathf.DeltaAngle(transform.rotation.eulerAngles.z, needed_angle);
            transform.eulerAngles = Vector3.forward * needed_angle;
        }
    }
    public void direct_to(Vector2 in_aim) {
        this.transform.direct_to(in_aim);
    }
    public void set_direction(float in_direction) {
        this.transform.set_direction(in_direction);
    }
    public Quaternion rotation {
        get {
            return transform.rotation;
        }
    }
    public Transform transform {
        get {
            return game_object.transform;
        }
    }
    
}

}
}