using rvinowise.unity.actions;
using rvinowise.unity.extensions;
using UnityEngine;
using rvinowise.unity.geometry2d;


namespace rvinowise.unity {

public class Landmine_supertool: 
    Supertool_description 
{

    public Expendable_equipment expendable_equipment;

    
    public override void start_using_action(Humanoid user) {
        Use_planting_tool_supertool.create(
            user,
            tool,
            expendable_equipment
        ).start_as_root(user.action_runner);
    }
}
}