using System;
using System.Collections;
using System.Collections.Generic;
using geometry2d;
using UnityEngine;
using rvinowise.units.parts.limbs.arms;


namespace rvinowise.units.parts.tools {
public abstract class Tool: MonoBehaviour
{

    public Holding_place main_holding;
    public Holding_place second_holding;

    public Degree direction = new Degree(0);
    
    
    protected SpriteRenderer spriteRenderer;

    
    public virtual void Awake() {
        init_components();
        init_holding_places();
    }

    private void init_components() {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }


    protected virtual void init_holding_places() {
        main_holding = new Holding_place(this);
    }
    

    public void deactivate() {
        gameObject.SetActive(false);
    }
    public void activate() {
        gameObject.SetActive(true);
    }
}

public class Holding_place {
    public Vector2 attachment_point = Vector2.zero;
    public Hand_gesture grip_gesture = Hand_gesture.Relaxed;
    public Degree grip_direction = new Degree(0);
    public Quaternion grip_direction_quaternion {
        get { return grip_direction.to_quaternion(); }
    }
    public Tool tool;

    public Holding_place(Tool in_tool) {
        tool = in_tool;
    }
    
    public Vector2 position {
        get{
            return tool.transform.TransformPoint(attachment_point);
        }
    }
    public Quaternion rotation {
        get { return tool.transform.rotation * grip_direction_quaternion; }
    }

    
}

}