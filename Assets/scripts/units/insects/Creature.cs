using UnityEngine;


namespace rvinowise.unity {

[RequireComponent(typeof(PolygonCollider2D))]
public abstract class Creature: Unit
{
    /* IChildren_groups_host interface */
    public virtual ITransporter transporter { get; protected set; }
    public virtual IAttacker attacker { get; set; }
    
    
    /* Creature itself */
    public Divisible_body divisible_body;
    public Bleeding_body bleeding_body;

    

    protected virtual void Start() {
        create_equipment();
        
    }

    protected virtual void create_equipment() {}
    
  

}


}