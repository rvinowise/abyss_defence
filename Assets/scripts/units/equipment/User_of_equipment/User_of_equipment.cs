using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using rvinowise;
using rvinowise.units;
using rvinowise.units.equipment;
using rvinowise.rvi.contracts;
using rvinowise.units.debug;
using units.equipment.transport;

namespace rvinowise.units.equipment
{
public partial class User_of_equipment:MonoBehaviour
{

    public GameObject game_object {
        get { return this.gameObject; }
    }
    public ITransporter transporter { get; set; }
    public IWeaponry weaponry { get; set; }

    public IList<Equipment_controller> equipment_controllers {
        get {
            return _equipment_controllers;
        }
        private set {
            _equipment_controllers = (List<Equipment_controller>)value;
            foreach (Equipment_controller equipment_controller in _equipment_controllers) {
                equipment_controller.add_to_user(this);
            }
        }
    }
    private IList<Equipment_controller> _equipment_controllers = new List<Equipment_controller>();
    
    private void Awake()
    {
        debug = new Debug(this);
        debug.increase_counter();
    }
    public void add_equipment_controller(Equipment_controller tool_controller) {
        tool_controller.add_to_user(this);
        _equipment_controllers.Add(tool_controller);
    }
    public T add_equipment_controller<T>() where T: 
        Equipment_controller, new()   
    {
        T new_tool_controller = new T();
        new_tool_controller.add_to_user(this);
        _equipment_controllers.Add(new_tool_controller);
        return new_tool_controller;
    }

    public void add_equipment_controllers_after(User_of_equipment src_user) {
        transporter = src_user.transporter?.copy_empty_into(this) as ITransporter;
        weaponry = src_user.weaponry?.copy_empty_into(this) as IWeaponry;
        
    }

    /*private void copy_array_of_equipment_controllers(User_of_equipment src_user) {
        foreach (var src_equipment_controller in src_user.equipment_controllers) {
            equipment_controllers.Add(src_equipment_controller.copy_empty_into(this));
        }
    }*/


    

    private void FixedUpdate() {
        foreach (var equipment_controller in equipment_controllers) {
            equipment_controller.update();
        }
    }

    private void OnDestroy() {
        debug.decrease_counter();
    }

    public void remove_empty_controllers() {
        List<Equipment_controller> new_tool_controllers = new List<Equipment_controller>(equipment_controllers.Count);
        for (int i_tool_controller = 0; i_tool_controller < equipment_controllers.Count; i_tool_controller++) {
            Equipment_controller tool_controller = equipment_controllers[i_tool_controller];
            if (tool_controller.tools.Any()) {
                new_tool_controllers.Add(tool_controller);
            }
            else {
                destroy_tool_controller(tool_controller);
            }
        }
        equipment_controllers = new_tool_controllers;

    }

    private void destroy_tool_controller(
        Equipment_controller tool_controller) 
    {
        /*IEquipment_controller component_tool_controller =
            tool_controller as IEquipment_controller;
        if (component_tool_controller) {
            Destroy(component_tool_controller);
        }*/
    }

    

    void OnDrawGizmos() {
        foreach (Equipment_controller equipment_controller in equipment_controllers) {
            equipment_controller.on_draw_gizmos();
        }
    }
    
    public class Debug: Debugger {
        //private User_of_tools user_of_tools;
        protected override ref int count {
            get { return ref _count; }
        }
        static protected int _count = 0;

        public Debug(User_of_equipment in_user):base(in_user) {
        }

    }
    public Debug debug;
    
}

}

