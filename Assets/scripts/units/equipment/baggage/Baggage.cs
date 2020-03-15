using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise;
using rvinowise.units.parts.limbs;
using rvinowise.units.parts.weapons;


namespace rvinowise.units.parts {

public class Baggage: Turning_element {

    public List<Gun> items { get; set; } = new List<Gun>();

    public float entering_span = 30f;

    public int ensure_borders(int index) {
        if (Math.Abs(index) > items.Count) {
            index = items.Count % index;
        }
        if (index < 0) {
            index = items.Count - index;
        }
        return index;
    }
}
}