

namespace rvinowise.unity.actions {

public class Idle: Action_leaf {


    
    public static Idle create(   
        Actor actor
    ) {
        Idle action = (Idle)object_pool.get(typeof(Idle));
        action.add_actor(actor);
        
        return action;
    }

    // protected override void on_start_execution() {
    //     base.on_start_execution();
    //     if (actors[0] is ITransporter transporter) {
    //         transporter.command_batch.
    //     }
    //     
    // }


}
}