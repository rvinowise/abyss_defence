using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise;
using rvinowise.units.control;
using rvinowise.units.equipment;
using units.control;
using units.equipment.arms;
using units.equipment.transport;



namespace rvinowise.units.human {

using rvinowise.units.equipment;


[RequireComponent(typeof(PolygonCollider2D))]
public class Human: 
    MonoBehaviour 
{
 
    private Intelligence intelligence;
    private User_of_equipment user_of_equipment;
    private Head head;
    
    protected virtual void Awake()
    {
        user_of_equipment = GetComponent<User_of_equipment>();
        intelligence = new Player_control(transform, user_of_equipment);
        
        equip();
    }
    
    public void equip() {
        intelligence.transporter = create_transporter();
        intelligence.weaponry = create_weaponry();
        head = new Head();
    }

    protected ITransporter create_transporter() {
        return new units.equipment.humanoid.Legs(user_of_equipment);
    }
    protected IWeaponry create_weaponry() {
        return new Arm_controller(user_of_equipment);
    }
    //protected abstract IWeaponry get_weaponry();

    void FixedUpdate() {
        intelligence.update();
    }
    
    

}
}