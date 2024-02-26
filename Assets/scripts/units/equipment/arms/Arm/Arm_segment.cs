using UnityEngine;
using rvinowise.unity.extensions;


namespace rvinowise.unity {

public class Arm_segment: Segment {

    [HideInInspector]
    public Quaternion desired_idle_rotation;
    
}
}