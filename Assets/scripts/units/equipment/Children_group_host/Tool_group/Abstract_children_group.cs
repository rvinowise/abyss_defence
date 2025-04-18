﻿using System.Collections.Generic;
using System.Linq;
using rvinowise.unity.debug;
using UnityEngine;


namespace rvinowise.unity
{

/*
represents a coherent system of several objects, 
which work together under control of this object:
creeping_legs, Weapons etc. 
*/
public abstract class Abstract_children_group:
    MonoBehaviour,
    IChildren_group 
{

    public abstract IEnumerable<IChild_of_group> get_children();

    public IList<IChild_of_group> children_stashed_from_copying {
        get;
        private set;
    }

    public virtual void hide_children_from_copying() {
        foreach (var child in get_children()) {
            child.transform.SetParent(null,false);
        }
        children_stashed_from_copying = get_children().Where(child => child != null).ToList();
        
    }

    public bool has_child(IChild_of_group in_compound_object) {
        return get_children().Any(tool => tool == in_compound_object);
    }


    public abstract void add_child(IChild_of_group in_child);

    public virtual void distribute_data_across(
        IEnumerable<IChildren_group> new_controllers
    ) {}

    public virtual void shift_center(Vector2 in_shift) {
        foreach (IChild_of_group child in get_children()) {
            child.transform.localPosition += (Vector3)in_shift;
        }
    }



    

    
}

}