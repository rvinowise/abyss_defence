using UnityEngine;
using rvinowise.contracts;
using rvinowise.unity.units.parts.transport;
using rvinowise.unity.geometry2d;
using rvinowise.unity.units.parts.actions;
using Action = rvinowise.unity.units.parts.actions.Action;

namespace rvinowise.unity.units.parts.humanoid {

public class Legs: 
    MonoBehaviour,
    ITransporter
{
    
    /* ITransporter interface */

    //[HideInInspector]
    public transport.Transporter_commands command_batch { get; } = new transport.Transporter_commands();
    
    
    public void Update() {
        execute_commands();
    }

    protected void execute_commands() {
        move_in_direction(command_batch.moving_direction_vector);
        rotate_to_direction(command_batch.face_direction_degrees);
        
    }

    [SerializeField]
    private float _possible_rotation  = 400f;
    public float possible_rotation {
        get { return _possible_rotation; }
        set { _possible_rotation = value;}
    }
    [SerializeField]
    private float _possible_impulse  = 1f;
    public float possible_impulse {
        get { return _possible_impulse; }
        set { _possible_impulse = value;}
    }

    public Quaternion direction_quaternion {
        get { return turning_element.rotation; }
    }


    /* legs itself */
    public Rigidbody2D rigid_body;
    public Turning_element turning_element;
    public GameObject user;

    private float acceleration = 0.339f * rvinowise.Settings.scale;


    public void move_in_direction(Vector2 direction) {
        Vector2 force = direction * (possible_impulse * Time.deltaTime);
        rigid_body.MovePosition(rigid_body.position + force);
    }
    
    public void rotate_to_direction(float face_direction) {
        turning_element.target_rotation = Directions.degrees_to_quaternion(face_direction);
        //turning_element.rotation_acceleration = possible_rotation;
        turning_element.rotate_to_desired_direction();
    }

    #region IActor
    public Action current_action { get; set; }
    public void on_lacking_action() {
    }

    private Action_runner action_runner;
    
    public void init_for_runner(Action_runner action_runner) {
        this.action_runner = action_runner;
    }

    #endregion
}
}