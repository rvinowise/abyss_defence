using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using rvinowise.unity.extensions;
using rvinowise.contracts;
using rvinowise.unity.actions;
using Action = rvinowise.unity.actions.Action;


namespace rvinowise.unity {

public class Rocket :
    MonoBehaviour {

    public Rigidbody2D moved_body;
    public float acceleration_speed;
    public float initial_impulse = 2f;

    public Transform attachment;
    
    public GameObject smoke_trail;
    public GameObject propulsion_fire;

    public Explosive_body explosive_body;
    //public Computer_intelligence intelligence;
    public Homing_missile homing_missile;
    
    private void Awake() {
        homing_missile = GetComponent<Homing_missile>();
        explosive_body = GetComponent<Explosive_body>();
        homing_missile.enabled = false;
        
        
        smoke_trail.SetActive(false);
        propulsion_fire.SetActive(false);
    }

    public bool is_flying() {
        return homing_missile.enabled;
    }
    public void launch() {
        moved_body.AddForce(initial_impulse*transform.rotation.to_vector());
        homing_missile.enabled = true;
        activate_propulsion_effects();

    }

    public void activate_propulsion_effects() {
        smoke_trail.SetActive(true);
        propulsion_fire.SetActive(true);
    }

    public void FixedUpdate() {
        //moved_body.AddForce(acceleration_speed*transform.rotation.to_vector());
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (is_flying()) {
            explosive_body.on_start_dying();
        }
    }

}
}