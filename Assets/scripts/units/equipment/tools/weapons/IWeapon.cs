using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise;


namespace rvinowise.units.parts.weapons {

public interface IWeapon {

    void pull_trigger();

    float time_to_readiness();

}
}