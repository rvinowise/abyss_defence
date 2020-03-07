using rvinowise.units;
using rvinowise.units.parts.weapons;
using UnityEngine;

namespace rvinowise.units.parts.weapons.guns.Pistol {

public class Pistol: 
    Gun
{
    
    /* Child interface */
    public override Transform host {
        get { return _host; }
        set { _host = value; }
    }
    private Transform _host;

    private GameObject game_object;
    
    /* Gun interface */
    public override void fire() {
    }

    public override Object get_projectile() {
        throw new System.NotImplementedException();
    }

    public override float time_to_readiness() {
        return 0;
    }
    
}
}