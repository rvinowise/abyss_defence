using System;
using System.Collections;
using System.Collections.Generic;
using MoreLinq.Extensions;
using UnityEngine;
using rvinowise.rvi.contracts;
using rvinowise.units.parts;
using rvinowise.units.parts.transport;

namespace rvinowise.units.parts.limbs.arms {

public class Arm_controller: 
    IChildren_group
    ,IWeaponry
{
    
    /* IChildren_group interface */
    protected IChildren_groups_host host;
    
    public IEnumerable<Child> children {
        get { return arms; }
    }


    public GameObject game_object;
    public Transform transform {
        get { return game_object.transform; }
    }
    

    public void update() {
        foreach (Arm arm in arms) {
            arm.update();
        }
    }


    public void add_child(Child child) {
        Contract.Requires(child is Arm);
        arms.Add(child as Arm);
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
    
    public IList<Arm> arms = new List<Arm>();
    
    public IList<Held_tool> held_tools = new List<Held_tool>();

    public ITransporter transporter;


    public Arm_controller(IChildren_groups_host in_user, ITransporter in_transporter) {
        host = in_user;
        game_object = in_user.game_object;
        transporter = in_transporter;
    }

    public Arm_controller(GameObject in_user, ITransporter in_transporter) {
        game_object = in_user;
        transporter = in_transporter;
    }
    
    

    public Arm_controller() : base() { }
    
    
    

}
}