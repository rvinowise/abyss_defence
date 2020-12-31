using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise.unity.extensions;

using rvinowise;
using rvinowise.unity.units.parts;
using rvinowise.unity.units.parts.sensors;
using rvinowise.unity.units.parts.transport;
using Baggage = rvinowise.unity.units.parts.Baggage;
using rvinowise.contracts;
using System.Linq;

namespace rvinowise.unity.units.control {

/* asks tool controllers what they can do,
 orders them some actions based on this information,
 they do these actions later in the same step */
public abstract class Intelligence: 
MonoBehaviour 
{
    
    public Baggage baggage;
    public ISensory_organ sensory_organ;
    public ITransporter transporter { get; set; }
    public IWeaponry weaponry{ get; set; }
    public float last_rotation;

    
    void Start() {
        sensory_organ = GetComponent<ISensory_organ>(); 
        transporter = GetComponent<ITransporter>();
        Contract.Requires(
            GetComponentsInChildren<Baggage>().Length <= 1, "which baggage should the Intelligence use?"
        );
        baggage = GetComponentInChildren<Baggage>();

    }
    protected virtual void FixedUpdate() {
        read_input();
    }

    protected virtual void read_input() {
        
    }
    
    protected void save_last_rotation(Quaternion needed_direction) {
        float angle_difference = transform.rotation.degrees_to(needed_direction).degrees;
        if (Mathf.Abs(angle_difference) > (float) Mathf.Epsilon) {
            last_rotation = angle_difference;
        }
    }
}
}