using UnityEngine;


namespace rvinowise.unity.actions {

public class Defender_start_defending:Action_leaf {

    private IDefender defender;
    private Transform target;
    
    public static Defender_start_defending create(
        IDefender in_defender,
        Transform in_target
    ) {
        var action = (Defender_start_defending)object_pool.get(typeof(Defender_start_defending));

        action.defender = in_defender;
        action.target = in_target;

        return action;
    }


    protected override void on_start_execution() {
        base.on_start_execution();
        
        defender.start_defence(target, mark_as_completed);
    }
}

}