using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise;
using geometry2d;


namespace rvinowise.units.parts {

public interface IExecute_commands {

    Command_batch command_batch { get; }

    void update();
    //void execute_commands();

}
}