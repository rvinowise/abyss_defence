using rvinowise.units;
using rvinowise.units.equipment.weapons;
using UnityEngine;

namespace units.equipment.weapons.guns.Pistol {

public class Pistol: 
    Weapon
{
    
    /* Tool interface */
    public override Transform host {
        get { return _host; }
        set { _host = value; }
    }
    private Transform _host;

    private GameObject game_object;
    
    /* Weapon interface */
    public override void fire() {
    }

    public override float time_to_readiness() {
        return 0;
    }
}
}