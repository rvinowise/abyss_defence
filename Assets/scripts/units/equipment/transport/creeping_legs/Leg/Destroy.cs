using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using rvinowise.units.parts.limbs;
using rvinowise.units.parts.limbs.legs;
using UnityEngine;
using static UnityEngine.Object; //Destroy()

namespace rvinowise {

public static partial class Deleter {
    public static void Destroy(Leg leg) {
        UnityEngine.Object.Destroy(leg.femur.game_object);
        UnityEngine.Object.Destroy(leg.tibia.game_object);
    }
}

}