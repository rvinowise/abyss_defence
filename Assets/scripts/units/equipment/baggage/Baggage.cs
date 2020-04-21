using System;
using System.Collections;
using System.Collections.Generic;
using extesions;
using UnityEngine;
using rvinowise;
using rvinowise.units.parts.limbs;
using rvinowise.units.parts.tools;
using rvinowise.units.parts.weapons.guns;


namespace rvinowise.units.parts {

public class Baggage: Turning_element {

    public List<Tool> items { get; set; } = new List<Tool>();
    public Dictionary_of_containers<Gun, Tool> ammos = new Dictionary_of_containers<Gun, Tool>();

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

    public void insert_ammo_for_gun(Gun gun, Tool in_ammo) {
        ammos.insert(gun.GetType(), in_ammo);
    }


    public Tool retrieve_ammo_for_gun(Gun in_gun) {
        return ammos.get(in_gun.GetType());
    }
}
}