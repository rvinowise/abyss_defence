using rvinowise.unity.actions;
using UnityEngine;
using rvinowise.unity.extensions;


namespace rvinowise.unity {

[RequireComponent(typeof(Turning_element))]
public class Turning_sensor: 
    Sensor_child_of_group,
    ISensory_organ 
{
    
    public Turning_element turning_element;

    public Transform attention_target;

    public System.Action on_focused_on_target;

  
    
    public override void pay_attention_to_target(Transform target) {
        attention_target = target;
    }

    public override bool is_focused_on_target() {
        return turning_element.at_desired_rotation();
    }

    private bool has_attention_target() {
        return attention_target != transform;
    }

    protected void Awake() {
        //base.Awake();
        turning_element = GetComponent<Turning_element>();
        if (attention_target == null) {
            attention_target = transform; // to avoid costly null comparison
        }
    }

    protected void Update() {
        if (has_attention_target()) {
            turning_element.set_target_rotation(
                ((Vector2) attention_target.position - (Vector2)transform.position).to_quaternion()
            );
        }
        else {
            turning_element.set_target_rotation(transform.parent.rotation);
        }
        
        turning_element.rotate_to_desired_direction();
        if (turning_element.at_desired_rotation()) {
            on_focused_on_target?.Invoke();
        }
    } 
    
    #region IActor
    

    public void on_lacking_action() {
        Idle.create(this).start_as_root(actor.action_runner);
    }

    #endregion

#if UNITY_EDITOR
    protected void OnDrawGizmos() {
        if (Application.isPlaying) {
            rvinowise.unity.debug.Debug.DrawLine_simple(
                transform.position, 
                transform.position + turning_element.get_target_rotation() * Vector2.right * 0.3f,
                Color.blue,
                3
            );
        }
    }
#endif
    
}


}