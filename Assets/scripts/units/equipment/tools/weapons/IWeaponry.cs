using System;
using System.Collections;
using UnityEngine;
using rvinowise.unity.extensions;


namespace rvinowise.unity.units.parts {
public interface IWeaponry: IExecute_commands {
    bool can_reach(Transform target);
    void attack(Transform target);
}
}