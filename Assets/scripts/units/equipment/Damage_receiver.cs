using UnityEngine;
using rvinowise.unity.units.parts;
using rvinowise.unity.effects.persistent_residue;
using rvinowise.unity.units.control;
using rvinowise.unity.units.parts.limbs.creeping_legs;
using rvinowise.unity.units.parts.weapons.guns.common;

namespace rvinowise.unity.units {

public class Damage_receiver: MonoBehaviour {

    public delegate void EvendHandler(Damage_receiver unit);
    public event EvendHandler on_destroyed;

    private Divisible_body divisible_body;
    private ILeaving_persistent_residue leaving_residue;

    private Creeping_leg_group leg_group;
    private Intelligence intelligence;
    
    private bool needs_to_die;
    private float dying_moment = float.MaxValue;
    private float dying_time = 1f;
    void Awake() {
        divisible_body = GetComponent<Divisible_body>();
        leaving_residue = GetComponent<ILeaving_persistent_residue>();
        leg_group = GetComponent<Creeping_leg_group>();
        intelligence = GetComponent<Intelligence>();

        if (divisible_body != null) {
            divisible_body.on_polygon_changed+= prepare_to_death;
        }
    }

    public void prepare_to_death() {
        needs_to_die = true;
    }
    private void destroy() {
        leaving_residue.leave_persistent_residue();
        
        notify_about_destruction();
        Destroy(gameObject);
    }

    public void notify_about_destruction() {
        if (on_destroyed != null) {
            on_destroyed(this);
        }
    }

    void FixedUpdate() {
        if (needs_to_die) {
            destroy();
        }
    }

    void Update() {
        if (Time.time >= dying_moment) {
            destroy();
        }
    }

    void OnCollisionEnter2D(Collision2D collision) {
        
        if (collision.get_damaging_projectile() is Projectile damaging_projectile ) {
            //destroy();
            intelligence.start_dying(damaging_projectile);
            dying_moment = Time.time + dying_time;
        }
    } 
}
}