using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise;
using geometry2d;


namespace rvinowise.units.parts.actions {

public interface IPerform_actions {

    Action current_action { set; get; }

    void set_root_action(Action in_root_action);

    //void start_new_action(Action in_action);

    //void set_action(Action in_action);

    //void prepare_for_execution_of_root_action(Action root_action);
}
}