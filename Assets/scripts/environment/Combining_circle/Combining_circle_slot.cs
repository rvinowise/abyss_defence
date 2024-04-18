using System.Collections;
using System.Collections.Generic;
using rvinowise.unity.extensions;
using rvinowise.unity.geometry2d;
using UnityEngine;


namespace rvinowise.unity {
public class Combining_circle_slot : MonoBehaviour {

    public Transform content;
    public bool filled;

    public bool is_filled() {
        //return content != null;
        return filled;
    }
    
    public void set_empty() {
        content = null;
        filled = false;
    }
}

    
}