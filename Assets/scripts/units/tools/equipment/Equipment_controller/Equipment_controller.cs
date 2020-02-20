using System.Collections.Generic;
using UnityEngine;

namespace rvinowise.units.equipment
{

/* this class is needed only to make the User_of_tools component
necessary with the Tool_controller. Otherwise direct implementing 
IEquipment_controller is enough.
 
represents a coherent system of several objects, 
which work together under control of this object:
Legs, Weapons etc. 
*/
//[RequireComponent(typeof(User_of_tools))]
public abstract class Equipment_controller:
    IEquipment_controller
    
{
    protected User_of_equipment userOfEquipment; //host

    public Equipment_controller() {
        debug = new Debug(this);
        debug.increase_counter();
    }

    ~Equipment_controller() {
        debug.decrease_counter();
    }
    
    public Transform transform {
        get {
            return userOfEquipment.transform;
        }
    }
    
    public abstract IEnumerable<Tool> tools {
        get;
    }
    public void add_to_user(User_of_equipment in_user) {
        userOfEquipment = in_user;
    }

    public abstract IEquipment_controller copy_empty_into(User_of_equipment dst_host);

    public abstract void add_tool(Tool tool);

    public abstract void init();
    public abstract void update();

    public class Debug: debug.Debugger {
        protected override ref int count {
            get { return ref _count; }
        }
        protected int _count = 0;

        public Debug(Equipment_controller in_controller):base(in_controller) {
        }
    }

    public Debug debug;
}

}