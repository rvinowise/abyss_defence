using System;
using System.Collections;
using System.Collections.Generic;
using MoreLinq.Extensions;
using UnityEngine;
using rvinowise;
using rvinowise.rvi.contracts;
using rvinowise.units;
using rvinowise.units.equipment;
using rvinowise.units.equipment.limbs;
using rvinowise.units.equipment.limbs.arms;


namespace units.equipment.arms {

public class Arm_controller: 
    Equipment_controller
    ,IWeaponry
{
    
    /* Equipment_controller interface */
    public override IEnumerable<Child> tools {
        get { return arms; }
    }
    public IList<Arm> arms = new List<Arm>();
    
    IList<Held_weapon> held_weapons = new List<Held_weapon>();
    public override IEquipment_controller copy_empty_into(User_of_equipment dst_host) {
        throw new NotImplementedException();
    }

    public override void update() {
        
    }

    public override void add_tool(Child child) {
        Contract.Requires(child is Arm);
        arms.Add(child as Arm);
    }

    protected override Command_batch new_command_batch() {
        throw new NotImplementedException();
    }
    
    /* IWeaponry interface */
    public void fire() {
    }

    public void shoot(Transform target) {
        var fastest_weapon = get_weapon_reaching_faster(target);
        fastest_weapon.shoot(target);
    }

    private Held_weapon get_weapon_reaching_faster(Transform target) {
        Held_weapon fastest_weapon = held_weapons.MinBy(
            held_weapon => held_weapon.time_to_shooting(target)
        ).First();
        return fastest_weapon;
    }

    

    /* Arm_controller itself */
    public Arm_controller(User_of_equipment user) : base(user) {
        
    }
    
}
}