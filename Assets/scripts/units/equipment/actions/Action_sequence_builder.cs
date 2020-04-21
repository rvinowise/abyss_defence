using System.Collections.Generic;

namespace rvinowise.units.parts.actions {
public interface Action_sequence_builder {

    Action current_child_action {
        set;
        get;
    }

    Action new_next_child {
        get;
        set;
    }

    void start_next_child_action(Action new_action);


}
}