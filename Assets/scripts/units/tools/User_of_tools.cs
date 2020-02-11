using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;

namespace units
{
public class User_of_tools:MonoBehaviour
    ,IUser_of_tools
{
    /* interface of IUser_of_tools */
    public IEnumerable<ITool_controller> tool_controllers {
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
            if (tool_controller is T) {
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

}

}

