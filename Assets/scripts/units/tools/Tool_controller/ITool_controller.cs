using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using UnityEngine;


namespace units
{

/* represents a coherent system of several objects, 
which work together under control of this object:
Legs, Weapons etc. */
public interface ITool_controller
{
    IEnumerable<Tool> tools {
        get;
    }

    void add_tool(Tool tool);
    void init();
    void update();
}

}
