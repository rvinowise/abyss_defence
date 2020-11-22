using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise.unity.extensions;

using rvinowise;


namespace rvinowise.unity.units.parts.weapons {

public interface IWeapon {

    void pull_trigger();

    float time_to_readiness();

}
}