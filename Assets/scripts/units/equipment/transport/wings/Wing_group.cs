using System;
using System.Collections.Generic;
using System.Linq;
using rvinowise.unity.extensions;
using rvinowise.unity.geometry2d;
using rvinowise.unity.actions;
using UnityEngine;
using Action = rvinowise.unity.actions.Action;


namespace rvinowise.unity {

public class Wing_group:
    Flying_transporter
    ,ITransporter
    ,IChildren_group
{

    public List<Wing> wings = new List<Wing>();

    
    #region Children_group
    public IEnumerable<IChild_of_group> get_children() {
        return wings;
    }

    public void add_child(IChild_of_group in_child) {
        if (in_child as Wing is {} wing) {
            wings.Add(wing);
            wing.transform.SetParent(transform, false);
        }
    }

    public IList<IChild_of_group> children_stashed_from_copying { get; private set; }
    public void hide_children_from_copying() {
        foreach (var child in get_children()) {
            child.transform.SetParent(null,false);
        }
        children_stashed_from_copying = get_children().Where(child => child != null).ToList();
        wings.Clear();
    }

    public void shift_center(Vector2 in_shift) {
        foreach (IChild_of_group child in get_children()) {
            child.transform.localPosition += (Vector3)in_shift;
        }
    }
    
    public void distribute_data_across(IEnumerable<IChildren_group> new_controllers) {
        foreach (var controller in new_controllers) {
            if (controller as Wing_group is {} wing_group) {
                if (wing_group.wings.Count == 1) {
                    var single_wing = wing_group.wings.First();
                    single_wing.fold();
                    wing_group.acceleration_speed = 0;
                }
            }
        }
    }

    
    
    #endregion //Children_group
}

}