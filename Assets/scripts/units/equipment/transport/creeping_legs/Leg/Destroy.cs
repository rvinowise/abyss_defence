using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using rvinowise.unity.units.parts.limbs;
using rvinowise.unity.units.parts.limbs.creeping_legs;
using UnityEngine;
using rvinowise.unity.extensions;

using static UnityEngine.Object; //Destroy()

namespace rvinowise {

public static partial class Deleter {
    public static void Destroy(Leg leg) {
        UnityEngine.Object.Destroy(leg.femur.gameObject);
        UnityEngine.Object.Destroy(leg.tibia.gameObject);
    }
}

}