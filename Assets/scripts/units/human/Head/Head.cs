using UnityEngine;
using rvinowise.unity.extensions;
using rvinowise.unity.units.parts.sensors;


namespace rvinowise.unity.units.parts.head {

public class Head: Turning_element, ISensory_organ {

    public Transform attention_target;

    protected void Start() {
        attention_target = rvinowise.unity.ui.input.Player_input.instance.cursor.transform;
    }
    public static Head create() {
        GameObject game_object = new GameObject();
        game_object.AddComponent<SpriteRenderer>();
        var new_component = game_object.add_component<Head>();
        return new_component;
    }
    

    public void pay_attention_to(Vector3 point) {
        target_rotation = ((Vector2)point - (Vector2)position).to_quaternion();
    }

    protected void Update() {
        pay_attention_to(attention_target.position);
        base.rotate_to_desired_direction();
        preserve_possible_rotations();
    } 

    protected override void OnDrawGizmos() {
        base.OnDrawGizmos();
        if (Application.isPlaying) {
            rvinowise.unity.debug.Debug.DrawLine_simple(
                transform.position, 
                transform.position + target_rotation * Vector2.right * 0.3f,
                Color.blue,
                3
            );
        }
    }
}


}