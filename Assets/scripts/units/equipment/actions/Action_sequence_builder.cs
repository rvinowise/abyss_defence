using System.Collections.Generic;

namespace rvinowise.unity.units.parts.actions {
public interface Action_sequence_builder {

    Action current_child_action_setter {
        set;
        //get;
    }
    Action current_child_action { get; }

    Action new_next_child {
        get;
        set;
    }

    void start_next_child_action(Action new_action);


}
}