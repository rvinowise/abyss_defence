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
using rvinowise.unity.ui;
using UnityEngine.Serialization;

namespace rvinowise.unity.units.parts {

public class Baggage: 
Turning_element
{

    public List<Toolset> tool_sets = new List<Toolset>();
    public List<Tool> tools = new List<Tool>();
    public float entering_span = 30f;
    public Dictionary<Ammo_compatibility, int> tool_to_ammo = 
        new Dictionary<Ammo_compatibility, int>();

    public int ensure_borders(int index) {
        if (Math.Abs(index) > tool_sets.Count) {
            index = tool_sets.Count % index;
        }
        if (index < 0) {
            index = tool_sets.Count - index;
        }
        return index;
    }

    public void add_tool(Tool tool) {
        tool.transform.SetParent(transform, false);
        tool.transform.localRotation = Quaternion.identity;
        tool.transform.localPosition = Vector2.zero;
        tool.deactivate();
        tools.Add(tool);
    }

    public void remove_tool(Tool tool) {
        tools.Remove(tool);
        tool.activate();
    }

    

    public void change_ammo_qty(Ammo_compatibility in_compatibility, int in_qty) {
        Contract.Assert(in_compatibility != null);
        tool_to_ammo.TryGetValue(in_compatibility, out var old_qty);
        int new_qty = old_qty + in_qty;
        //Contract.Assert(new_qty >=0);
        tool_to_ammo[in_compatibility] = new_qty;
        
        if (on_ammo_changed != null) {
            on_ammo_changed();
        }
    }
    
    public int fetch_ammo_qty(Ammo_compatibility in_compatibility, int needed_qty) {
        tool_to_ammo.TryGetValue(in_compatibility, out var available_qty);
        int retrieved_qty = Math.Max(available_qty, needed_qty);
        change_ammo_qty(in_compatibility, retrieved_qty);
        return retrieved_qty;
    }
    public int check_ammo_qty(Ammo_compatibility in_compatibility) {
        return fetch_ammo_qty(in_compatibility, 0);
    }


    public Ammunition get_ammo_object_for_tool(Tool in_tool) {
        Ammunition ammo_prefab = in_tool.ammo_prefab;
        Ammunition ammo = ammo_prefab.instantiate<Ammunition>();
        ammo.rounds_qty = fetch_ammo_qty(in_tool.ammo_compatibility, ammo_prefab.max_rounds_qty);
        Contract.Ensures(ammo.rounds_qty > 0);
        
        return ammo;
    }
    
    
    
    void OnCollisionEnter2D(Collision2D collision) {
        
    }

    public delegate void EventHandler();
    public event EventHandler on_ammo_changed;

    public event EventHandler on_tools_changed;

    /*public Ammunition retrieve_ammo_for_gun(
        Gun in_gun,
        Ammo_type in_type
    ) {
        Ammunition ammo = Instantiate();
        return ammos.get(in_gun.GetType());
    }*/
}
}