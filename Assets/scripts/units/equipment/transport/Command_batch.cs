using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise;


namespace rvinowise.units.equipment.transport {

public class Command_batch: equipment.Command_batch {
    
    public float moving_direction_degrees/* {
        get { return _moving_direction_degrees;}
        private set { _moving_direction_degrees = value; }
    }
    private float _moving_direction_degrees*/;

    public Vector2 moving_direction_vector/* {
        get { return _moving_direction_vector; }
        private set { _moving_direction_vector = value; }
    }
    private Vector2 _moving_direction_vector*/;

    public float face_direction_degrees/* {
        get { return _face_direction_degrees;}
        private set { _face_direction_degrees = value; }
    }
    private float _face_direction_degrees*/;
    
}
}