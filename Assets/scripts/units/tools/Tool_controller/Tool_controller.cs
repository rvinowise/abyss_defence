using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;


namespace units
{

/* represents a coherent system of several objects, 
which work together under control of this object:
Legs, Weapons etc. */
[RequireComponent(typeof(User_of_tools))]
public abstract class Tool_controller:MonoBehaviour
    ,ITool_controller
    
{
    public abstract IEnumerable<Tool> tools {
        get;
    }

    public abstract void add_tool(Tool tool);
}

}