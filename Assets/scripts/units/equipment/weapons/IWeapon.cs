using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise;


namespace rvinowise.units.parts.weapons {

public interface IWeapon {

    void fire();

    float time_to_readiness();

}
}