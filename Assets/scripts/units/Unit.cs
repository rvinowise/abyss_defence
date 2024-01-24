using rvinowise.unity.units.parts;
using rvinowise.unity.units.control;

namespace rvinowise.unity.units {

public class Unit: Turning_element {
    protected Intelligence intelligence;

    protected override void Awake() {
        intelligence = GetComponent<Intelligence>();
    }


}


}