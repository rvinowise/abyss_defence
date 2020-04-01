using System;
using System.Collections;
using System.Collections.Generic;
using MoreLinq.Extensions;
using UnityEngine;
using rvinowise.rvi.contracts;
using rvinowise.units.parts;

namespace rvinowise.units.parts.limbs.arms {

public class Arm_controller: 
    Children_group
    ,IWeaponry
{
    
    /* Children_group interface */
    public override IEnumerable<Child> children {
        get { return arms; }
    }
    
    public override IChildren_group copy_empty_into(IChildren_groups_host dst_host) {
        throw new NotImplementedException();
    }


    public Command_batch command_batch { get; }

    public void update() {
        foreach (Arm arm in arms) {
            arm.update();
        }
    }


    public override void add_child(Child child) {
        Contract.Requires(child is Arm);
        arms.Add(child as Arm);
    }


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
    
    public IList<Arm> arms = new List<Arm>();
    
    public IList<Held_tool> held_tools = new List<Held_tool>();
    
    
    public Arm_controller(IChildren_groups_host user) : base(user) {
        
    }

    public Arm_controller() : base() { }

}
}