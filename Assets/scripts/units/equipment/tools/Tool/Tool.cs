using System;
using System.Collections;
using System.Collections.Generic;
using geometry2d;
using UnityEngine;
using rvinowise.units.parts.limbs.arms;


namespace rvinowise.units.parts.tools {
public abstract class Tool: MonoBehaviour
{

    public virtual float weight { set; get; }
    
    public Holding_place main_holding;
    public Holding_place second_holding;
    
    public Animator animator;

    
    protected virtual void Awake() {
        init_components();
        init_holding_places();
    }

    protected virtual void init_components() {
        animator = gameObject.GetComponent<Animator>();
        //rigid_body = gameObject.GetComponent<Rigidbody2D>();
        
    }


    protected virtual void init_holding_places() {
        main_holding = Holding_place.main(this);
        
    }
    

    public void deactivate() {
        gameObject.SetActive(false);
    }
    public void activate() {
        gameObject.SetActive(true);
    }
}

public interface IHave_velocity {
    Vector2 velocity { get; set; }
}



}
