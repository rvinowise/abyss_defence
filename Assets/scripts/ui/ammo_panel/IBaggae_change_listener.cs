using System.Collections;
using System.Collections.Generic;
using rvinowise.unity.units.parts;
using rvinowise.unity.units.parts.tools;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace rvinowise.unity.ui {
public interface IBaggae_change_listener
{
    void update_ammo(Tool in_tool, int change);
    void update_available_tools(Tool in_tool, int change);
}

}