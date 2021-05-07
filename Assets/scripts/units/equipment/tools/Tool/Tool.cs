using System;
using System.Collections;
using System.Collections.Generic;
using rvinowise.unity.geometry2d;
using UnityEngine;
using rvinowise.unity.extensions;

using rvinowise.unity.units.parts.limbs.arms;
using rvinowise.contracts;
using rvinowise.unity.maps;

namespace rvinowise.unity.units.parts.tools {
public abstract class Tool: MonoBehaviour
{

    public virtual float weight { set; get; }
    
    public Holding_place main_holding;
    public Holding_place second_holding;
    
    public Animator animator;
    
    public Ammunition ammo_prefab;
    public Ammo_compatibility ammo_compatibility;
    public int ammo_qty;
    public int max_ammo_qty;
    public int ammo_value = 1;
    
    [HideInInspector]
    public Saved_physics last_physics = new Saved_physics();
    protected virtual void Awake() {
        init_components();
        init_holding_places();
    }

    

    protected virtual void init_components() {
        animator = gameObject.GetComponent<Animator>();
        //rigid_body = gameObject.GetComponent<Rigidbody2D>();
        
    }

    protected virtual void init_holding_places() {
        var holding_places = GetComponentsInChildren<Holding_place>();
        
        foreach(Holding_place holding_place in holding_places) {
            if (holding_place.is_main) {
                main_holding = holding_place;
            } else {
                second_holding = holding_place;
            }
        }
        Contract.Assert(main_holding != null);
    }
    
    public virtual void hold_by(Hand in_hand) {
        transform.set_z(in_hand.held_object_local_z);
    }

    private static float ground_z = 0;
    public virtual void drop_from_hand() {
        transform.set_z(Map.instance.ground_z);
    }

    public void deactivate() {
        gameObject.SetActive(false);
    }
    public void activate() {
        gameObject.SetActive(true);
    }

    protected virtual void LateUpdate()
    {
        last_physics.position = transform.position;
    }

    
}




}
