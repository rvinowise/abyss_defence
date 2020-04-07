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
    
    
    protected SpriteRenderer sprite_renderer;
    protected Rigidbody2D rigid_body;

    
    protected virtual void Awake() {
        init_components();
        init_holding_places();
    }

    protected virtual void init_components() {
        sprite_renderer = gameObject.GetComponent<SpriteRenderer>();
        rigid_body = gameObject.GetComponent<Rigidbody2D>();
        
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

public class Holding_place {
    public Vector2 place_on_tool = Vector2.zero;
    public Hand_gesture grip_gesture = Hand_gesture.Relaxed;
    public Degree grip_direction = new Degree(0);
    public Quaternion grip_direction_quaternion {
        get { return grip_direction.to_quaternion(); }
    }
    public Tool tool;
    public bool is_main;
    public Arm holding_arm {
        get { return _holding_arm; }
        set {
            _holding_arm = value;
            if (is_main) {
                tool.gameObject.transform.parent = _holding_arm.hand.transform;
            }
        }
    }
    public Arm _holding_arm;

    public IHave_velocity holder {
        get { return holding_arm.hand; }
    }

    public Holding_place(Tool in_tool) {
        tool = in_tool;
    }

    public static Holding_place main(Tool in_tool) {
        Holding_place holding_place = new Holding_place(in_tool);
        holding_place.is_main = true;
        return holding_place;
    }
    public static Holding_place secondary(Tool in_tool) {
        Holding_place holding_place = new Holding_place(in_tool);
        holding_place.is_main = false;
        return holding_place;
    }
    
    public Vector2 position {
        get{
            return tool.transform.TransformPoint(place_on_tool);
        }
    }
    public Quaternion rotation {
        get { return tool.transform.rotation * grip_direction_quaternion; }
    }
    
}

}
