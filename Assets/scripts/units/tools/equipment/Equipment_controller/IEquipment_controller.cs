using System.Collections.Generic;

namespace rvinowise.units.equipment
{

/* represents a coherent system of several objects, 
which work together under control of this object:
Legs, Weapons etc. */
public interface IEquipment_controller
{
    IEnumerable<Tool> tools {
        get;
    }
    
    void add_tool(Tool tool);
    
    /* i need this function only for a generic adder (constructors can't have parameters there)*/
    void add_to_user(User_of_equipment in_user);

    IEquipment_controller copy_empty_into(User_of_equipment dst_host);
    
    void init();
    void update();
}


}
