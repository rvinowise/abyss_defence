using UnityEngine;
using rvinowise.unity.ui.input;

namespace rvinowise.unity.units.control {

/* autoaiming can target this object */
public class Targetable: 
MonoBehaviour 
{
  
    void OnMouseOver()
    {
        if (transform != Player_input.instance.player.transform) {
            Player_input.instance.player.aim_at(transform);
        }
    }
}
}