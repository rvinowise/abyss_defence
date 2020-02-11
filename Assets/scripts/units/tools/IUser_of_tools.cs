using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using geometry2d;


namespace units
{

/* The only IUser_of_tools is the Component User_of_tools. delete this? */
public interface IUser_of_tools
{
    IEnumerable<ITool_controller> tool_controllers {
        get;
    }

    void add_tool_controller(ITool_controller tool_controller);
}

}