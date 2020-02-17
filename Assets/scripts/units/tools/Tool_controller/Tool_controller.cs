using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using UnityEngine;


namespace units
{

/* this class is needed only to make the User_of_tools component
necessary with the Tool_controller. Otherwise direct implementing 
ITool_controller is enough.
 
represents a coherent system of several objects, 
which work together under control of this object:
Legs, Weapons etc. 
*/
[RequireComponent(typeof(User_of_tools))]
public abstract class Tool_controller:MonoBehaviour
    ,ITool_controller
    
{
    public abstract IEnumerable<Tool> tools {
        get;
    }

    public abstract void add_tool(Tool tool);

    public abstract void init();
    public abstract void update();
}

}