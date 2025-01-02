using System.Collections.Generic;
using rvinowise.unity.geometry2d;
using rvinowise.unity;
using rvinowise.unity.extensions;
using UnityEngine;
using UnityEngine.Assertions;


namespace rvinowise.unity.actions {

public class Attach_unit_parts: Action_leaf {

    private Combining_circle combining_circle;

    private Combining_circle_slot body_slot;
    private Combining_circle_slot legs_slot;
    private Combining_circle_slot head_slot;
    public static Attach_unit_parts create(
        Combining_circle circle,
        Combining_circle_slot head_slot,
        Combining_circle_slot body_slot,
        Combining_circle_slot legs_slot
    ) {
        var action = (Attach_unit_parts) object_pool.get(typeof(Attach_unit_parts));

        action.combining_circle = circle;
        action.head_slot = head_slot;
        action.body_slot = body_slot;
        action.legs_slot = legs_slot;
        
        return action;
    }


    protected override void on_start_execution() {
        
        base.on_start_execution();

        attach_slots_content(
            head_slot,
            body_slot,
            legs_slot
        );
    }

    private Intelligence attach_slots_content(
        Combining_circle_slot head_slot,
        Combining_circle_slot body_slot,
        Combining_circle_slot limbs_slot
    ) {
        Assert.IsNotNull(body_slot.content);
        Assert.IsNotNull(limbs_slot.content);
        Assert.IsNotNull(head_slot.content);
        
        var attachable_body = body_slot.content.GetComponent<Attachable_body>();
        
        var attachable_limbs = limbs_slot.content.GetComponent<Attachable_limbs>();
        if (attachable_limbs != null) {
            attachable_limbs.attach_limbs_to_body(attachable_body);
        }
        var head_group = head_slot.content.GetComponentInChildren<Children_group>();
        if (head_group != null) {
            attach_head_to_body(attachable_body, head_group);
        }
        
        attachable_body.transform.parent = null;
        var unit_intelligence = attachable_body.GetComponent<Computer_intelligence>();
        unit_intelligence.team = combining_circle.team;
        unit_intelligence.action_runner.start_fallback_actions();
        unit_intelligence.init_devices();
        unit_intelligence.move_towards_best_target();
        unit_intelligence.enabled = true;
        attachable_body.GetComponent<Divisible_body>()?.Awake();
        
        delete_attaching_helpers(attachable_body);
        
        return unit_intelligence;
    }

    private void attach_head_to_body(Attachable_body body, Children_group head_group) {
        if (body.intelligence.transporter is Creeping_leg_group creeping_leg_group) {
            var head_legs = head_group.GetComponentsInChildren<ALeg>(); //e.g. squid head has legs
            foreach (var leg in head_legs) {
                creeping_leg_group.add_child(leg);
            }
        }
        
        head_group.transform.parent = body.transform;
        head_group.transform.localRotation = Vector2.right.to_quaternion();
        head_group.transform.localPosition = Vector3.zero;

        //body.intelligence.attacker = head_group.GetComponentInChildren<IAttacker>();
        //body.intelligence.sensory_organ = head_group.GetComponentInChildren<ISensory_organ>();
        
        //head.transform.parent = body.transform;
        var head_transform = head_group.GetComponentInChildren<Head>().transform;
        head_transform.localPosition = body.head_attachment.localPosition;
        head_transform.localRotation = body.head_attachment.localRotation;
    }

    private void delete_attaching_helpers(Attachable_body attachable_body) {
        foreach (var helper in attachable_body.disposed_after_attachment) {
            Object.Destroy(helper.gameObject);
        }
        Object.Destroy(attachable_body);
    }

}

}