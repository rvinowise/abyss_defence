using System;
using System.Collections;
using UnityEngine;

namespace rvinowise.units.parts {
public interface IWeaponry: IEquipment_controller {
    void fire();

    void shoot(Transform target);
}
}