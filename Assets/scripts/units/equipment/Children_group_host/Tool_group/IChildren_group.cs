using System.Collections.Generic;

namespace rvinowise.units.parts
{

/* represents a coherent system of several objects, 
which work together under control of this object:
creeping_legs, Weapons etc. */
public interface IChildren_group
{
    IEnumerable<ICompound_object> children {
        get;
    }
    
    void add_child(ICompound_object compound_object);
    
    /* i need this function only for a generic adder (constructors can't have parameters there)*/
    void add_to_user(IChildren_groups_host in_user);

    
    void init();
}


}
