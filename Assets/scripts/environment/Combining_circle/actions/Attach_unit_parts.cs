using System.Collections.Generic;
using rvinowise.unity.geometry2d;
using rvinowise.unity;
using rvinowise.unity.extensions;
using UnityEngine;
using UnityEngine.Assertions;


namespace rvinowise.unity.actions {

public class Attach_unit_parts: Action_leaf {

    private Team team;
    private Intelligence.Evend_handler on_destroyed_listener;
    
    private Combining_circle_slot body_slot;
    private Combining_circle_slot legs_slot;
    private Combining_circle_slot head_slot;
    public static Attach_unit_parts create(
        Team team,
        Combining_circle_slot head_slot,
        Combining_circle_slot body_slot,
        Combining_circle_slot legs_slot,
        Intelligence.Evend_handler on_destroyed_listener
    ) {
        var action = (Attach_unit_parts) object_pool.get(typeof(Attach_unit_parts));

        action.team = team;
        action.head_slot = head_slot;
        action.body_slot = body_slot;
        action.legs_slot = legs_slot;
        action.on_destroyed_listener = on_destroyed_listener;
        
        return action;
    }


    protected override void on_start_execution() {
        
        base.on_start_execution();

        
    }

    public override void update() {
        base.update();
        
        attach_slots_content(
            head_slot,
            body_slot,
            legs_slot
        );
        
        mark_as_completed();
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
        var unit_intelligence = attachable_body.GetComponent<Computer_intelligence>();
        
        var attachable_limb_groups = limbs_slot.content.GetComponentsInChildren<Attachable_limbs>();
        foreach (var attachable_limb_group in attachable_limb_groups) {
            attach_all_actors_to_intelligence(unit_intelligence, attachable_limb_group);
            attachable_limb_group.attach_appendages_to_body(attachable_body);
        }
        unit_intelligence.init_devices();
        
        var attachable_head = head_slot.GetComponentInChildren<Attachable_head>();
        if (attachable_head != null) {
            attach_all_actors_to_intelligence(unit_intelligence, attachable_head);
            var attached_group = attach_head_to_body(attachable_body, attachable_head);
        }
        
        if (head_slot.GetComponentInChildren<Attachable_guns>() is {} attachable_guns) {
            attach_all_actors_to_intelligence(unit_intelligence, attachable_guns);
            var attached_group = attach_guns_to_body(attachable_body, attachable_guns);
        }
        
        attachable_body.transform.parent = null;
        
        attach_all_team_members_to_team(attachable_body, team);
        attach_all_tools_to_intelligence(attachable_body, unit_intelligence);
        
        team.add_unit(unit_intelligence);
        unit_intelligence.action_runner.start_fallback_actions();
        unit_intelligence.init_devices();
        unit_intelligence.fill_lacking_devices_with_empty_devices();
        unit_intelligence.action_runner.init_actors();
        unit_intelligence.move_towards_best_target();
        unit_intelligence.enabled = true;
        unit_intelligence.on_destroyed += on_destroyed_listener;
        attachable_body.GetComponent<Divisible_body>()?.Awake();
        
        delete_attaching_helpers(attachable_body);
        
        return unit_intelligence;
    }

    private Abstract_children_group attach_head_to_body(Attachable_body attachable_body, Attachable_head attachable_head) {

        Abstract_children_group head_group = attachable_head.GetComponentInChildren<Abstract_children_group>();
        
        if (attachable_body.intelligence.transporter is Creeping_leg_group creeping_leg_group) {
            var head_legs = head_group.GetComponentsInChildren<ALeg>(); //e.g. squid head has legs
            foreach (var leg in head_legs) {
                creeping_leg_group.add_child_on_other_transform(leg);
            }
        }
        
        head_group.transform.parent = attachable_body.transform;
        head_group.transform.localRotation = Vector2.right.to_quaternion();
        head_group.transform.localPosition = Vector3.zero;
        
        var head_transform = attachable_head.head.transform;
        head_transform.localPosition = attachable_body.head_attachment.localPosition;
        head_transform.localRotation = attachable_body.head_attachment.localRotation;

        return head_group;
    }
    
    private Abstract_children_group attach_guns_to_body(Attachable_body attachable_body, Attachable_guns attachable_guns) {

        Abstract_children_group gun_group = attachable_guns.GetComponentInChildren<Abstract_children_group>();
        
        gun_group.transform.parent = attachable_body.transform;
        gun_group.transform.localRotation = Vector2.right.to_quaternion();
        gun_group.transform.localPosition = Vector3.zero;

        attach_gun(attachable_guns.gun_l, attachable_body.gun_l_attachment);
        attach_gun(attachable_guns.gun_r, attachable_body.gun_r_attachment);

        return gun_group;
    }

    private void attach_gun(Transform gun, Transform attachment_point) {
        gun.transform.localPosition = attachment_point.localPosition;
        gun.transform.localRotation = attachment_point.localRotation;
    }

    private void delete_attaching_helpers(Attachable_body attachable_body) {
        foreach (var helper in attachable_body.disposed_after_attachment) {
            Object.Destroy(helper.gameObject);
        }
        Object.Destroy(attachable_body);
    }

    
    public static void attach_all_actors_to_intelligence(Intelligence intelligence, Component attached_object) {
        foreach (var actor in attached_object.GetComponentsInChildren<Actor>()) {
            actor.action_runner = intelligence.action_runner;
        }
    }
    
    public static void attach_all_team_members_to_team(
        Component unit_root,
        Team in_team
    ) {
        foreach (var member in unit_root.GetComponentsInChildren<ITeam_member>()) {
            member.attach_to_team(in_team);
        }
    }
    public static void attach_all_tools_to_intelligence(
        Component unit_root,
        Intelligence in_intelligence
    ) {
        foreach (var tool in unit_root.GetComponentsInChildren<IUsed_by_intelligence>()) {
            tool.attach_to_intelligence(in_intelligence);
        }
    }
}

}