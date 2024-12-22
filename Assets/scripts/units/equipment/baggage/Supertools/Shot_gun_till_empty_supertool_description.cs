using System;
using rvinowise.unity.actions;
using rvinowise.unity.extensions;
using UnityEngine;
using rvinowise.unity.geometry2d;


namespace rvinowise.unity {

public class Shot_gun_till_empty_supertool_description: 
    Supertool_description
{

    private IGun gun;
    private IReloadable reloadable;
    public Ammo_compatibility ammo_compatibility;

    private void Awake() {
        gun = tool.GetComponent<IGun>();
        reloadable = tool.GetComponent<IReloadable>();
    }

    public override void start_using_action(Humanoid user) {
        Fire_gun_till_empty.create(
            user,
            gun,
            tool,
            reloadable,
            ammo_compatibility
        ).start_as_root(user.action_runner);
    }
}
}