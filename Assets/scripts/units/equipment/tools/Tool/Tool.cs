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

    public Degree direction = new Degree(0);
    
    
    public SpriteRenderer sprite_renderer;
    public Animator animator;
    //protected Rigidbody2D rigid_body;

    
    protected virtual void Awake() {
        init_components();
        init_holding_places();
    }

    protected virtual void init_components() {
        sprite_renderer = gameObject.GetComponent<SpriteRenderer>();
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
