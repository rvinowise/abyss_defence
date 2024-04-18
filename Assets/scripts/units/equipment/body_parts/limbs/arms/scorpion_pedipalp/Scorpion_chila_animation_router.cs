using System;
using System.Collections.Generic;
using UnityEngine;

using rvinowise.contracts;
using rvinowise.unity.actions;
using rvinowise.unity.extensions;
using rvinowise.unity.extensions.attributes;
using rvinowise.unity.geometry2d;
using Action = rvinowise.unity.actions.Action;

namespace rvinowise.unity {


public class Scorpion_chila_animation_router:MonoBehaviour
{

    public Scorpion_chila chila;

    [called_in_animation]
    public void on_closed() {
        chila.on_closed();
    }
    
    [called_in_animation]
    public void on_opened() {
        chila.on_opened();
    }
}

}