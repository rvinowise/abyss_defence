using System;
using System.Collections.Generic;

using UnityEngine;
using rvinowise.unity.extensions;
using rvinowise.unity.extensions.pooling;
using rvinowise.contracts;


namespace rvinowise.unity {

[Serializable]
public class Power_tool_ammo {
    public Tool tool;
    public int amount;

}
    
public class Baggage: 
Turning_element
{

    public List<Toolset> tool_sets = new List<Toolset>();
    public List<Tool> tools = new List<Tool>();
    public float entering_span = 30f;
    public Dictionary<Ammo_compatibility, int> tool_to_ammo = 
        new Dictionary<Ammo_compatibility, int>();

    public List<Power_tool_ammo> power_tools = new List<Power_tool_ammo>();
    public int current_powertool;
    
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


    public Ammunition get_ammo_object_for_gun(Gun in_gun) {
        Ammunition ammo_prefab = in_gun.ammo_prefab;
        Ammunition ammo = ammo_prefab.instantiate<Ammunition>();
        ammo.rounds_qty = fetch_ammo_qty(in_gun.ammo_compatibility, ammo_prefab.max_rounds_qty);
        Contract.Ensures(ammo.rounds_qty > 0);
        
        return ammo;
    }


    public Tool retrieve_current_powertool() {
        var current_tool_ammo = power_tools[current_powertool];
        if (current_tool_ammo.amount > 0) {
            var powertool_prefab = current_tool_ammo.tool;
            current_tool_ammo.amount -= 1;
            var retrieved_powertool = powertool_prefab.instantiate<Tool>(this.transform.position, this.transform.rotation);
            retrieved_powertool.deactivate();
            return retrieved_powertool;
        }
        return null;
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