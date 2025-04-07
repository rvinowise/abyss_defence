using System;
using UnityEngine;
using rvinowise.unity.extensions;
using rvinowise.unity.geometry2d;

using Random = UnityEngine.Random;


namespace rvinowise.unity {

public interface IBleeding_body {


    void create_splash(
        Vector2 in_position,
        Vector2 in_impulse
    );

    public void receive_damage(
        Vector2 contact_point,
        Vector2 impact_impulse,
        Vector2 impact_normal,
        float strenght
    );
}
}