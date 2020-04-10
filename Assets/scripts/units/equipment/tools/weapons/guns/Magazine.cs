using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise;
using geometry2d;
using rvinowise.rvi.contracts;


namespace rvinowise.units.parts.weapons.guns {

public class Magazine : MonoBehaviour {

    public virtual int max_rounds_qty { get; set; }
    [SerializeField] public int rounds_qty;

    [SerializeField] public Bullet bullet_prefab;

    public Magazine() { }

    public Bullet retrieve_round() {
        if (rounds_qty > 0) {
            rounds_qty--;
            return get_bullet_object();
        }
        return null;
    }

    public bool empty() {
        Contract.Requires(rounds_qty >=0, "number of rounds can't be negative");
        return rounds_qty == 0;
    }

    protected Bullet get_bullet_object() {
        return Instantiate(bullet_prefab, transform.position, transform.rotation);
    }
}

}