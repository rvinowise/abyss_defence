using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise;
using geometry2d;
using rvinowise.units.parts.tools;


namespace rvinowise.units.parts.limbs.arms {

[RequireComponent(typeof(Animator))]
public class Hand:Segment,
IHave_velocity 
{

    
    public Side side = Side.LEFT;
    
    private Animator animator;
    
    public Hand_gesture gesture {
        get { return _gesture;}
        set {
            _gesture = value;
            animator.SetInteger("gesture", value.Value);
            tip = value.valuable_point;
            
            // since the tool is a child of the hand - its side-transformations are applied automatically 
            /*if (side == Side.RIGHT) {
                _tip.y = -_tip.y;
            }*/
        } 
    }
    private Hand_gesture _gesture;
    
    public Vector2 velocity { get; set; }
    public float linear_drag { get; set; } = 0.02f;

    
    public int bottom_sorting_order {
        get {
            return bottom_sprite.sortingOrder;
        }
    }
    private SpriteRenderer bottom_sprite;

    public Transform valuable_point;
    protected void Awake() {
        base.Awake();
        animator = GetComponent<Animator>();
        bottom_sprite = transform.Find("bottom")?.GetComponent<SpriteRenderer>();
        valuable_point = transform.Find("valuable_point")?.transform;
    }
    
    
    public static Hand create(string in_name, GameObject in_prefab) {
        GameObject game_object = GameObject.Instantiate(in_prefab);
        game_object.AddComponent<SpriteRenderer>();
        var new_component = game_object.add_component<Hand>();
        return new_component;
    }
    
    public override void mirror_from(limbs.Segment src) {
        base.mirror_from(src);
        if (src is arms.Hand src_hand) {
            side = src_hand.side.mirror();
        }
    }
}
}