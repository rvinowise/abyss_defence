using UnityEngine;

namespace rvinowise.unity {

public class Damage_receiver: MonoBehaviour {

   
    

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
        leaving_residue?.leave_persistent_residue();
        
        Destroy(gameObject);
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