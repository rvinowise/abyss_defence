using System;
using UnityEngine;

namespace rvinowise.unity {

/* autoaiming can target this object */
public class Targetable: 
MonoBehaviour, IDestructible 
{

    public Team team;

    private void Start() {
        //team.add_targetable(this);
    }

    void OnMouseOver()
    {
        if (transform == Player_input.instance.player.transform) {
            return;
        }
        
        //Player_input.instance.player.arm_pair.set_explicit_target(this.transform);
    }

    private void OnMouseExit() {
        //Player_input.instance.player.arm_pair.remove_explicit_target(this.transform);
    }

    public void die() {
        team?.remove_targetable(this);
    }
}
}