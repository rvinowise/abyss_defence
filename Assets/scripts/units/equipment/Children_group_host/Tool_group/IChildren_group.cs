using System.Collections.Generic;
using UnityEngine;


namespace rvinowise.unity
{

/* represents a coherent system of several objects, 
which work together under control of this object:
creeping_legs, Weapons etc. */
public interface IChildren_group {
    IEnumerable<IChild_of_group> get_children();
    IList<IChild_of_group> children_stashed_from_copying {
        get;
    }

     void hide_children_from_copying();
    
    void add_child(IChild_of_group compound_object);

    void distribute_data_across(
        IEnumerable<IChildren_group> new_controllers
    );

    void shift_center(Vector2 in_shift);

    Transform transform { get; }
    //void init();
}


}
