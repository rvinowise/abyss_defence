using rvinowise.unity.units.parts.actions;
using UnityEngine;

namespace rvinowise.unity.units.parts.limbs {

    public interface ILimb:IActor {
        bool is_twisted_badly();

        void set_desired_directions_by_position(Vector2 target);
        void move_segments_towards_desired_direction();
        bool has_reached_aim();

        float get_reaching_distance();

        #region debug
        void draw_desired_directions();
        void draw_directions(Color in_color, float in_time=0.1f);
        #endregion
    }

}