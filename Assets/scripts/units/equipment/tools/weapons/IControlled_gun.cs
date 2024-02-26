using UnityEngine;


namespace rvinowise.unity {

public interface IControlled_gun: IGun {

    /* actions */

    void rotate_to(Vector2 direction);

    /* sensors */
    float possible_rotation_speed();

    float time_to_rotate_to(Vector2 needed_direction);

}
}