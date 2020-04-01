using System;
using System.Collections;
using UnityEngine;

namespace rvinowise.units.parts {
public interface IWeaponry: IExecute_commands {
    void fire();

    void shoot(Transform target);
}
}