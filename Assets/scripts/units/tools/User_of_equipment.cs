using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using UnityEngine;
using rvinowise;
using rvinowise.units;
using rvinowise.units.equipment;
using rvinowise.units.debug;

namespace rvinowise.units.equipment
{
public class User_of_equipment:MonoBehaviour
{
    private IList<IEquipment_controller> _equipment_controllers = new List<IEquipment_controller>();

    public IControl control;
    public ITransporter transporter { get; set; }
    public IWeaponry weaponry { get; set; }

    public IList<IEquipment_controller> equipment_controllers {
        get {
            return _equipment_controllers;
        }
        private set {
            _equipment_controllers = (List<IEquipment_controller>)value;
            foreach (IEquipment_controller equipment_controller in _equipment_controllers) {
                equipment_controller.add_to_user(this);
            }
        }
    }
    
    private void Awake()
    {
        debug = new Debug(this);
        debug.increase_counter();
    }
    public void add_equipment_controller(IEquipment_controller tool_controller) {
        tool_controller.add_to_user(this);
        _equipment_controllers.Add(tool_controller);
    }
    public T add_equipment_controller<T>() where T: 
        IEquipment_controller, new()   
    {
        T new_tool_controller = new T();
        new_tool_controller.add_to_user(this);
        _equipment_controllers.Add(new_tool_controller);
        return new_tool_controller;
    }

    public void copy_equipment_controllers_from(User_of_equipment src_user) {
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

    public void init_equipment_controllers() {
        foreach (IEquipment_controller equipment_controller in equipment_controllers) {
            equipment_controller.init();
        }
    }

    public void remove_empty_controllers() {
        List<IEquipment_controller> new_tool_controllers = new List<IEquipment_controller>(equipment_controllers.Count);
        for (int i_tool_controller = 0; i_tool_controller < equipment_controllers.Count; i_tool_controller++) {
            IEquipment_controller tool_controller = equipment_controllers[i_tool_controller];
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
        IEquipment_controller tool_controller) 
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

