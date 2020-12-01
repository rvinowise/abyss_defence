using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using rvinowise.unity.extensions;

using rvinowise;
using rvinowise.contracts;
using rvinowise.unity.units.parts.limbs;
using rvinowise.unity.units.parts.tools;
using rvinowise.unity.units.parts.weapons.guns;


namespace rvinowise.unity.units.parts {

public class Baggage: Turning_element {

    public List<Tool> items = new List<Tool>();
    public Dictionary_of_queues<System.Type, Ammunition> ammos = new Dictionary_of_queues<System.Type, Ammunition>();

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

    public void insert_ammo_for_gun(Gun gun, Ammunition in_ammo) {
        Contract.Assert(gun != null);
        Contract.Assert(in_ammo != null);
        ammos.add(gun.GetType(), in_ammo);
    }


    public Ammunition retrieve_ammo_for_gun(Gun in_gun) {
        
        return ammos.retrieve_one(in_gun.GetType());
    }
    
    /*public Ammunition retrieve_ammo_for_gun(
        Gun in_gun,
        Ammo_type in_type
    ) {
        Ammunition ammo = Instantiate();
        return ammos.get(in_gun.GetType());
    }*/
}
}