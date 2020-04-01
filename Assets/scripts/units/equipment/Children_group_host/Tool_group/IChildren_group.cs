using System.Collections.Generic;

namespace rvinowise.units.parts
{

/* represents a coherent system of several objects, 
which work together under control of this object:
Legs, Weapons etc. */
public interface IChildren_group
{
    IEnumerable<Child> children {
        get;
    }
    
    void add_child(Child child);
    
    /* i need this function only for a generic adder (constructors can't have parameters there)*/
    void add_to_user(IChildren_groups_host in_user);

    IChildren_group copy_empty_into(IChildren_groups_host dst_host);
    
    void init();
}


}
