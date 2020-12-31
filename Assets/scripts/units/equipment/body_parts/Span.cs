using System;
using rvinowise.unity.geometry2d;
using UnityEngine;
using rvinowise.unity.extensions;


namespace rvinowise.unity.units.parts.limbs {

[Serializable]
public struct Span {
    public Degree min; //maximum rotation to the left (counter-clockwise)
    public Degree max; //maximum rotation to the right (clockwise)
    public bool goes_through_switching_degrees;

    public Span(float val1, float val2) {
        min = Math.Min(val1, val2);
        max = Math.Max(val1, val2);
        goes_through_switching_degrees = false;
    }
   
    public Span init_for_direction(Degree in_direction) {
        //this = use_minus();
        this.min = min.use_minus();
        this.max = max.use_minus();
        
        goes_through_switching_degrees = 
            within_small_angle(
                in_direction
            ) ==
            within_small_angle(
                180f
            );
            
        var _min = min;
        var _max = max;
        min = Math.Min(_min,_max);
        max = Math.Max(_min,_max);
        return this;
    }
    private bool within_small_angle(float in_direction) {
        if (
            Math.Sign(min.angle_to(max)) != 
            Math.Sign(min.angle_to(in_direction))

        ) {
            return false;
        } else if (
            Math.Sign(max.angle_to(min)) != 
            Math.Sign(max.angle_to(in_direction))
        ) {
            return false;
        }

        return true;
    }
    public Span mirror() {
        return new Span(
            -max,
            -min
        ).use_minus();
    }

    public Span no_minus() {
        Span new_span = new Span();
        new_span.min = min.no_minus();
        new_span.max = max.no_minus();
        new_span.goes_through_switching_degrees = goes_through_switching_degrees;
        return new_span;
    }
    public Span use_minus() {
        Span new_span = new Span();
        new_span.min = min.use_minus();
        new_span.max = max.use_minus();
        new_span.goes_through_switching_degrees = goes_through_switching_degrees;
        return new_span;
    }

    public int sign_of_bigger_rotation() {
        if (Math.Abs(min) > Math.Abs(max)) {
            return Math.Sign(min);
        }
        return Math.Sign(max);
    }
    
    public unity.geometry2d.Side side_of_bigger_rotation() {
        Span span = this.use_minus();
        if (Math.Abs(span.min) > Math.Abs(span.max)) {
            return unity.geometry2d.Side.from_degrees(span.min);
        }
        return unity.geometry2d.Side.from_degrees(span.max);
    }

    public Degree bigger_direction() {
        return Math.Max(min,max);
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
