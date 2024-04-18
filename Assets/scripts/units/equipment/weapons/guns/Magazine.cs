using UnityEngine;
using rvinowise.unity.extensions;
using rvinowise.contracts;

namespace rvinowise.unity {

public class Magazine : Ammunition {

    [SerializeField] public Projectile projectile_prefab;


    protected override void Awake() {
        base.Awake();
    }
    
    /* Tool interface */
    

    public Projectile retrieve_round(Transform in_muzzle) {
        if (rounds_qty > 0) {
            rounds_qty--;
            return get_bullet_object(in_muzzle);
        }
        return null;
    }

    public bool empty() {
        Contract.Requires(rounds_qty >=0, "number of rounds can't be negative");
        return rounds_qty == 0;
    }

    protected Projectile get_bullet_object(Transform in_muzzle) {
        Projectile projectile = projectile_prefab.get_from_pool<Projectile>(
            in_muzzle.position, in_muzzle.rotation
        );
        return projectile;
    }
}

}