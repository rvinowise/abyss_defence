using rvinowise.unity.units.parts;
using rvinowise.unity.units.control;
using rvinowise.unity.units.gore;
using rvinowise.unity.units.parts.transport;
using UnityEngine;


namespace rvinowise.unity.units {

[RequireComponent(typeof(PolygonCollider2D))]
public abstract class Creature: rvinowise.unity.units.Unit
{
    /* IChildren_groups_host interface */
    public virtual ITransporter transporter { get; protected set; }
    public virtual IWeaponry weaponry { get; set; }
    
    
    /* Creature itself */
    public Divisible_body divisible_body;
    public Bleeding_body bleeding_body;

    

    protected virtual void Start() {
        create_equipment();
        
    }

    protected virtual void create_equipment() {}
    
  

}


}