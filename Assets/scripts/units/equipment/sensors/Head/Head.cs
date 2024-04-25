using rvinowise.unity.actions;
using UnityEngine;
using rvinowise.unity.extensions;


namespace rvinowise.unity {

public class Head: 
    Turning_element
    ,IActor_sensory_organ 
{

    public Transform attention_target;

    public System.Action on_focused_on_target;

    public void pay_attention_to_target(Transform target) {
        attention_target = target;
    }

    public bool is_focused_on_target() {
        return at_desired_rotation();
    }

    private bool has_attention_target() {
        return attention_target != transform;
    }

    protected override void Awake() {
        base.Awake();
        if (attention_target == null) {
            attention_target = transform; // to avoid costly null comparison
        }
    }

    protected void Update() {
        if (has_attention_target()) {
            set_target_rotation(
                ((Vector2) attention_target.position - (Vector2) position).to_quaternion()
            );
        }
        else {
            set_target_rotation(transform.parent.rotation);
        }
        
        rotate_to_desired_direction();
        if (at_desired_rotation()) {
            on_focused_on_target?.Invoke();
        }
    } 
    
    #region IActor

    public void init_for_runner(Action_runner action_runner) {
        this.action_runner = action_runner;
    }

    private Action_runner action_runner;

    public Action current_action { get; set; }
    public void on_lacking_action() {
        Idle.create(this).start_as_root(action_runner);
    }

    #endregion

#if UNITY_EDITOR
    protected override void OnDrawGizmos() {
        base.OnDrawGizmos();
        if (Application.isPlaying) {
            rvinowise.unity.debug.Debug.DrawLine_simple(
                transform.position, 
                transform.position + get_target_rotation() * Vector2.right * 0.3f,
                Color.blue,
                3
            );
        }
    }
#endif
    
}


}