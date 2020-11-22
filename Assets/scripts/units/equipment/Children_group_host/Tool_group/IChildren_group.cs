using System.Collections.Generic;

namespace rvinowise.unity.units.parts
{

/* represents a coherent system of several objects, 
which work together under control of this object:
creeping_legs, Weapons etc. */
public interface IChildren_group
{
    IEnumerable<ICompound_object> children {
        get;
    }
    IEnumerable<ICompound_object> children_stashed_from_copying {
        get;
    }

     void hide_children_from_copying();
    
    void add_child(ICompound_object compound_object);
    
    
    void init();
}


}
