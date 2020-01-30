using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace units {
namespace limbs {

/* Leg controller */
public class Leg_controller : MonoBehaviour
{
    public Sprite sprite_femur;
    public Sprite sprite_tibia;

    public List<Leg> legs {
        set {
            _legs = value;
            left_front_leg = value[0];
            /*right_front_leg = value[1];
            left_hind_leg = value[2];
            right_hind_leg = value[3];*/
        }
        get {
            return _legs;
        }
    }
    public Leg left_front_leg;
    public Leg right_front_leg;
    public Leg left_hind_leg;
    public Leg right_hind_leg;
    private List<Leg> _legs;

    void Start()
    {
        Legs_initializer.init_for_spider(this);
        set_aims();
    }
    private void set_aims() {
        foreach (Leg leg in legs) {
            leg.reset_aim();
        }
        /*rfloat scale = 0.2f;
        left_front_leg.holding_point = (Vector2)transform.position + new Vector2(1f,1f) * scale;
        ight_front_leg.holding_point = (Vector2)transform.position +new Vector2(1f,-1f)* scale;
        left_hind_leg.holding_point =(Vector2)transform.position +new Vector2(-1f,1f)* scale;
        right_hind_leg.holding_point = (Vector2)transform.position +new Vector2(-1f,-1f)* scale;*/
    }
    void Update()
    {
        move_legs();
        //attach_legs_to_attachment_points();
    }
    private void move_legs() {
        foreach (Leg leg in legs) {
            leg.move();
        }
    }
    
    private void attach_legs_to_attachment_points() {
        foreach (Leg leg in legs) {
            leg.attach_to_attachment_points();
        }
    }
    void OnDrawGizmos() {
        foreach (Leg leg in legs) {
            leg.draw_debug_gizmos();
        }
    }

    private void move_tips_towards_aim_points() {
        foreach (Leg leg in legs) {
            //leg.move_tip_towards_aim_point();
        }
    }
}

}
}