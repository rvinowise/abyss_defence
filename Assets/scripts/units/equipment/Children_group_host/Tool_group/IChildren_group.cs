using System.Collections.Generic;

namespace rvinowise.unity.units.parts
{

/* represents a coherent system of several objects, 
which work together under control of this object:
creeping_legs, Weapons etc. */
public interface IChildren_group
{
    IEnumerable<IChild_of_group> children {
        get;
    }
    IEnumerable<IChild_of_group> children_stashed_from_copying {
        get;
    }

     void hide_children_from_copying();
    
    void add_child(IChild_of_group compound_object);
    
    
    void init();
}


}
