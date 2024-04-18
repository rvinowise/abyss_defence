using UnityEngine;
using rvinowise.unity.geometry2d;
using rvinowise.unity.actions;
using UnityEngine.Serialization;


namespace rvinowise.unity {

public class Humanoid_legs: 
    MonoBehaviour,
    IActor_transporter
{
    
    /* ITransporter interface */


    [FormerlySerializedAs("_possible_rotation")] [SerializeField]
    private float possible_rotation  = 400f;
    public float get_possible_rotation() {
        return possible_rotation;
    }
    [SerializeField]
    private float possible_impulse  = 1f;
    public float get_possible_impulse() {
        return possible_impulse; 
    }

    public void set_moved_body(Turning_element body) {
        rigid_body = body.GetComponent<Rigidbody2D>();
    }
    public Turning_element get_moved_body() {
        return null;
    }
    
    /* legs itself */
    public Rigidbody2D rigid_body;
    public Turning_element turning_element;


    public void move_in_direction(Vector2 direction) {
        Vector2 force = direction * (possible_impulse * Time.deltaTime);
        rigid_body.AddForce(force);
    }


    #region ITransporter
    
    public void move_towards_destination(Vector2 destination) {
        Vector2 force = (destination-(Vector2)transform.position).normalized * (possible_impulse * Time.deltaTime);
        //rigid_body.AddForce(force);
        rigid_body.velocity = force;
    }
    
    public void face_rotation(Quaternion face_direction) {
        turning_element.set_target_rotation(face_direction);
        //turning_element.rotation_acceleration = possible_rotation;
        turning_element.rotate_to_desired_direction();
    }

    #endregion
    
    #region IActor
    public Action current_action { get; set; }
    public void on_lacking_action() {
        Idle.create(this).start_as_root(action_runner);
    }

    private Action_runner action_runner;
    
    public void init_for_runner(Action_runner action_runner) {
        this.action_runner = action_runner;
    }

    #endregion
}
}