using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using UnityEngine;

namespace units
{
public class User_of_tools:MonoBehaviour
    ,IUser_of_tools
{
    /* interface of IUser_of_tools */
    public IList<ITool_controller> tool_controllers {
        get {
            return _tool_controllers;
        }
        private set {
            _tool_controllers = (List<ITool_controller>)value;
        }
    }
    private List<ITool_controller> _tool_controllers;

    public void add_tool_controller(ITool_controller tool_controller) {
        _tool_controllers.Add(tool_controller);
    }

    /*public ITool_controller get_tool_controller_of_type(ITool_controller other) {
        foreach (ITool_controller tool_controller in tool_controllers) {
            if (tool_controller is other.GetType()) {
                return tool_controller;
            }
        }
        return null;
    }*/

    private void Awake()
    {
        _tool_controllers = new List<ITool_controller>(
            GetComponents<ITool_controller>()
        );
    }

    public void init_tool_controllers() {
        foreach (ITool_controller tool_controller in tool_controllers) {
            tool_controller.init();
        }
    }

    public void remove_empty_controllers() {
        List<ITool_controller> new_tool_controllers = new List<ITool_controller>(tool_controllers.Count);
        for (int i_tool_controller = 0; i_tool_controller < tool_controllers.Count; i_tool_controller++) {
            ITool_controller tool_controller = tool_controllers[i_tool_controller];
            if (tool_controller.tools.Any()) {
                new_tool_controllers.Add(tool_controller);
            }
            else {
                destroy_tool_controller(tool_controller);
            }
        }
        tool_controllers = new_tool_controllers;

    }

    private void destroy_tool_controller(
        ITool_controller tool_controller) 
    {
        Tool_controller component_tool_controller =
            tool_controller as Tool_controller;
        if (component_tool_controller) {
            Destroy(component_tool_controller);
        }
    }
}

}

