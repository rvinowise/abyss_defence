using UnityEngine;


namespace rvinowise.unity.actions {

public class Defender_finish_defending:Action_leaf {

    private IDefender defender;
    
    public static Defender_finish_defending create(
        IDefender in_defender
    ) {
        var action = (Defender_finish_defending)object_pool.get(typeof(Defender_finish_defending));

        action.defender = in_defender;

        return action;
    }


    protected override void on_start_execution() {
        base.on_start_execution();
        
        defender.finish_defence( mark_as_completed);
    }
}

}