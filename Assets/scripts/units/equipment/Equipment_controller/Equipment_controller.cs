using System.Collections.Generic;
using System.Linq;
using rvinowise.units.debug;
using UnityEngine;
using rvinowise.rvi.contracts;

namespace rvinowise.units.equipment
{

/*
represents a coherent system of several objects, 
which work together under control of this object:
Legs, Weapons etc. 
*/
public abstract class Equipment_controller:
    IEquipment_controller 
    
{
    protected User_of_equipment user_of_equipment; //host


    public GameObject game_object {
        get {
            return user_of_equipment.game_object;
        }
    }
    
    public abstract IEnumerable<Child> tools {
        get;
    }
    
    public Transform transform {
        get {
            return user_of_equipment.transform;
        }
    }

    private IList<equipment.Command_batch> command_batches;

    public void command(Command_batch command_batch) {
        command_batches.Add(command_batch);
    }

    /*protected Command_batch get_combined_commands() {
        Command_batch result = new_command_batch(); 
        foreach (var command_batch in command_batches) {
            
        }
    }*/

    protected abstract Command_batch new_command_batch();


    public bool has_tool(Child in_child) {
        return tools.Any(tool => tool == in_child);
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

    public abstract void add_tool(Child child);

    public virtual void init() { }

    public virtual void update() {
        execute_commands();
    }

    protected abstract void execute_commands();
        

    public virtual void distribute_data_across(
        IEnumerable<Equipment_controller> new_controllers) 
    {
        
    }

    

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