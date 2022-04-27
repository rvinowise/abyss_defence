using UnityEngine;

using rvinowise.unity.units.parts.limbs;

namespace rvinowise.unity.units.parts.limbs.creeping_legs {

public interface ILeg:
ILimb ,
IChild_of_group
{
    float provided_impulse{get;}
    bool is_up{get;}
    void put_down();
    float moving_offset_distance{get;}
    Vector2 optimal_position_standing{get;}
    Stable_leg_group stable_group{get;set;}

    void init();
    void set_desired_position(Vector2 position);
    bool hold_onto_ground();

    bool is_twisted_uncomfortably();
    void raise_up();

    #region debug
    void draw_positions();
    #endregion
    
}

}