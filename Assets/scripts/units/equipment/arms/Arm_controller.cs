using System;
using System.Collections;
using System.Collections.Generic;
using MoreLinq.Extensions;
using UnityEngine;
using rvinowise.unity.extensions;

using rvinowise.rvi.contracts;
using rvinowise.unity.units.parts;
using rvinowise.unity.units.parts.transport;

namespace rvinowise.unity.units.parts.limbs.arms {

public class Arm_controller: /*MonoBehaviour,*/
    Children_group
    ,IWeaponry
{
    
    /* IChildren_group interface */
    protected IChildren_groups_host host;
    
    public override IEnumerable<ICompound_object> children {
        get { return arms; }
    }


    public Transform transform {
        get { return gameObject.transform; }
    }


    protected virtual void Awake() {
        base.Awake();
        foreach (var arm in arms) {
            arm.controller = this;
        }
        transporter = GetComponent<ITransporter>();
    }

    public void update() {
        foreach (Arm arm in arms) {
            arm.update();
        }
    }


    public override void add_child(ICompound_object compound_object) {
        Contract.Requires(compound_object is Arm);
        arms.Add(compound_object as Arm);
    }

    public override void hide_children_from_copying() {
        children_stashed_from_copying = arms as ICollection<ICompound_object> ;
        arms = new List<Arm>();
    }

    public void add_to_user(IChildren_groups_host in_host) {
        host = in_host;
    }

    public void init() {
        
    }
    
    
    /* IExecute_commands interface */
    public Command_batch command_batch { get; }


    /* IWeaponry interface */
    public void fire() {
    }

    public void shoot(Transform target) {
        var fastest_weapon = get_weapon_reaching_faster(target);
        fastest_weapon.shoot(target);
    }

    private Held_tool get_weapon_reaching_faster(Transform target) {
        Held_tool fastest_tool = held_tools.MinBy(
            held_weapon => held_weapon.time_to_shooting(target)
        ).First();
        return fastest_tool;
    }


    /* Arm_controller itself */
    
    public List<Arm> arms = new List<Arm>();
    
    public List<Held_tool> held_tools = new List<Held_tool>();

    public ITransporter transporter;





    

}
}