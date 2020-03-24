using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise;


namespace rvinowise.units.parts.weapons {

public interface IControlled_weapon: IWeapon {

    /* actions */

    void rotate_to(Vector2 direction);

    /* sensors */
    float possible_rotation_speed();

    float time_to_rotate_to(Vector2 needed_direction);

}
}