using System;
using geometry2d;
using UnityEngine;

namespace rvinowise.units.parts.limbs {

public struct Span {
    /* possible rotation of a segment relative to it's attachment  */
    public readonly float min; //maximum rotation to the left (counter-clockwise)
    public readonly float max; //maximum rotation to the right (clockwise)

    public Span(float val1, float val2) {
        min = Math.Min(val1, val2);
        max = Math.Max(val1, val2);
    }

    public Span mirror() {
        return new Span(
            -max,
            -min
        );
    }

    public int sign_of_bigger_rotation() {
        if (Math.Abs(min) > Math.Abs(max)) {
            return Math.Sign(min);
        }
        return Math.Sign(max);
    }
    
    public geometry2d.Side side_of_bigger_rotation() {
        if (Math.Abs(min) > Math.Abs(max)) {
            return geometry2d.Side.from_degrees(min);
        }
        return geometry2d.Side.from_degrees(max);
    }
    
    public Span scaled(float scale) {
        float middle = (this.min + this.max)/2;
        float diff_to_min = Math.Abs(this.min) - Math.Abs(middle);
        float diff_to_max = Math.Abs(this.max) - Math.Abs(middle);
        Span span = new Span(
            middle+(scale * Math.Abs(diff_to_min)),
            middle-(scale * Math.Abs(diff_to_max))
        );
        return span;
    }

    public float degrees_closer_to(float in_degrees) {
        if (
            (Mathf.Abs(min)-Mathf.Abs(in_degrees)) < 
            (Mathf.Abs(max)-Mathf.Abs(in_degrees))
        )
        {
            return min;
        }
        return max;
    }
    
    public float degrees_in_direction(Side side) {
        
        if (side == Side.RIGHT)
        {
            return min;
        }
        return max;
    }
}

}
