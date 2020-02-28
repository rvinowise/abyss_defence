using System.Collections.Generic;
using rvinowise.units.debug;
using UnityEngine;

namespace rvinowise.units.equipment
{

/*
represents a coherent system of several objects, 
which work together under control of this object:
Legs, Weapons etc. 
*/
//[RequireComponent(typeof(User_of_tools))]
public abstract class Equipment_controller:
    IEquipment_controller
    
{
    protected User_of_equipment user_of_equipment; //host


    public GameObject game_object {
        get {
            return user_of_equipment.game_object;
        }
    }
    
    public abstract IEnumerable<Tool> tools {
        get;
    }
    
    
    protected Equipment_controller(User_of_equipment in_user) {
        debug = new Debug(this);
        debug.increase_counter();
        
        user_of_equipment = in_user;
        in_user.equipment_controllers.Add(this);
        
        init_components();
    }

    protected Equipment_controller() {
        debug = new Debug(this);
        debug.increase_counter();
        
        
    }
    
    protected virtual void init_components() { }
    

    ~Equipment_controller() {
        debug.decrease_counter();
    }
    
    public void add_to_user(User_of_equipment in_user) {
        user_of_equipment = in_user;
        init_components();
    }

    public abstract IEquipment_controller copy_empty_into(User_of_equipment dst_host);

    public abstract void add_tool(Tool tool);

    public abstract void init();
    public abstract void update();

    public class Debug: Debugger {
        protected override ref int count {
            get { return ref _count; }
        }
        static protected int _count = 0;

        public Debug(Equipment_controller in_controller):base(in_controller) {
        }
    }

    public Debug debug;

    

    

    public virtual void on_draw_gizmos() {
    
    }
    
}

}