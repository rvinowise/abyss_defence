using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise;
using rvinowise.units.parts.strategy;
using Action = rvinowise.units.parts.actions.Action;


namespace rvinowise.units.parts.strategy {

public interface IDo_actions {

    Action action { get; set; }

}
}