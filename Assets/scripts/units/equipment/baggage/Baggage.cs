using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise;
using rvinowise.units.parts.limbs;
using rvinowise.units.parts.tools;


namespace rvinowise.units.parts {

public class Baggage: Turning_element {

    public List<Tool> items { get; set; } = new List<Tool>();

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

    public void add_tool(Tool tool) {
        tool.deactivate();
        items.Add(tool);
    }

    public void remove_tool(Tool tool) {
        items.Remove(tool);
        tool.activate();
    }
}
}