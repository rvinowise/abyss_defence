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



namespace rvinowise.units {

using rvinowise.units.equipment;


[RequireComponent(typeof(PolygonCollider2D))]
public class Human: 
    MonoBehaviour 
{
 
    private Intelligence intelligence;
    
    
    protected virtual void Awake()
    {
        intelligence = new Player_control(transform);
        intelligence.user_of_equipment = GetComponent<User_of_equipment>();

        equip();
    }
    
    public void equip() {
        intelligence.transporter = create_transporter();
        intelligence.weaponry = create_weaponry();
        //weaponry = get_weaponry();
    }

    protected ITransporter create_transporter() {
        return new humanoid.Legs();
    }
    protected IWeaponry create_weaponry() {
        return new Arm_controller();
    }
    //protected abstract IWeaponry get_weaponry();

    void FixedUpdate() {
        intelligence.update();
    }
    
    

}
}