using UnityEngine;

namespace rvinowise.units.equipment.limbs {

public class Segment: Body_part {
    /* constant characteristics. they are written during construction */
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
    
    

    /* current characteristics */
    
    public Quaternion desired_direction { get; set; }

    public Segment(string name): base(name) {
        
    }

    public float length {
        get;
        private set;
    }
    public float sqr_length {
        get;
        private set;
    }

    
    
    
}

}