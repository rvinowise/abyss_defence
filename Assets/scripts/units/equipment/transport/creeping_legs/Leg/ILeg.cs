using UnityEngine;


namespace rvinowise.unity {

public interface ILeg:
ILimb,
IChild_of_group 
{
    float get_provided_impulse();
    bool is_up();
    void put_down();
    float get_moving_offset_distance();
    Vector2 get_optimal_position_standing();
    Stable_leg_group stable_group { get; set; }

    void set_desired_position(Vector2 position);
    bool hold_onto_ground();

    bool is_twisted_uncomfortably();
    void raise_up();

    #region debug
    void draw_positions();
    #endregion
    
}

}