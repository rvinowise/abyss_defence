using rvinowise.unity.actions;
using UnityEngine;

namespace rvinowise.unity {

    public interface ILimb:IActor {
        bool is_twisted_badly();

        void move_segments_towards_desired_direction();
        bool has_reached_aim();
        bool hold_onto_point(Vector2 target);

        Segment get_root_segment();
        
        #region debug
        void draw_desired_directions();
        void draw_directions(Color in_color);
        #endregion
    }

}