using System;
using System.Collections;
using System.Collections.Generic;
using MoreLinq.Extensions;
using UnityEngine;
using rvinowise.rvi.contracts;


namespace rvinowise.units.parts.limbs.arms {

public class Arm_controller: 
    Equipment_controller
    ,IWeaponry
{
    
    /* Equipment_controller interface */
    public override IEnumerable<Child> tools {
        get { return arms; }
    }
    
    public override IEquipment_controller copy_empty_into(User_of_equipment dst_host) {
        throw new NotImplementedException();
    }

    public override void update() {
        foreach (Arm arm in arms) {
            arm.update();
        }
    }

    protected override void execute_commands() {
        
    }

    public override void add_tool(Child child) {
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
    
    
    public Arm_controller(User_of_equipment user) : base(user) {
        
    }

    public Arm_controller() : base() { }

}
}