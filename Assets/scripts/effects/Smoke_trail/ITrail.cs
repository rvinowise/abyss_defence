using UnityEngine;

namespace rvinowise.unity {

public interface ITrail {

    void init_first_points(Vector2 position, Vector2 in_direction);
    void add_bend_at(Vector2 position);

    bool has_visible_parts();

}
}