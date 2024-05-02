using UnityEngine;
using rvinowise.unity.extensions;
using rvinowise.contracts;

namespace rvinowise.unity {

[RequireComponent(typeof(Ammunition))]
public class Magazine: MonoBehaviour {

    public Projectile projectile_prefab;

    public Ammunition ammunition;

    void Awake() {
        ammunition = GetComponent<Ammunition>();
    }
    public Projectile retrieve_round(Transform in_muzzle) {
        if (ammunition.rounds_qty > 0) {
            ammunition.rounds_qty--;
            return get_bullet_object(in_muzzle);
        }
        return null;
    }

    public bool is_empty() {
        Contract.Requires(ammunition.rounds_qty >=0, "number of rounds can't be negative");
        return ammunition.rounds_qty == 0;
    }

    private Projectile get_bullet_object(Transform in_muzzle) {
        Projectile projectile = projectile_prefab.get_from_pool<Projectile>(
            in_muzzle.position, in_muzzle.rotation
        );
        return projectile;
    }
}

}