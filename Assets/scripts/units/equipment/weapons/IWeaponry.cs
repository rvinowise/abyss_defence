using System;
using System.Collections;
using UnityEngine;

namespace rvinowise.units.equipment {
public interface IWeaponry: IEquipment_controller {
    void fire();

    void shoot(GameObject target);
}
}