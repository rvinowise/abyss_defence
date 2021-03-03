using System.Collections;
using System.Collections.Generic;
using rvinowise.unity.units.parts;
using rvinowise.unity.units.parts.tools;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace rvinowise.unity.ui {
public interface IBaggae_change_notifier
{
    void on_ammo_changed();
    void on_available_tools_changed();
}

}