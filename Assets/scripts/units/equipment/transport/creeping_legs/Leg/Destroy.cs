using rvinowise.unity.units.parts.limbs.creeping_legs;


//Destroy()

namespace rvinowise {

public static partial class Deleter {
    public static void Destroy(Leg2 leg) {
        UnityEngine.Object.Destroy(leg.femur.gameObject);
        UnityEngine.Object.Destroy(leg.tibia.gameObject);
    }
}

}