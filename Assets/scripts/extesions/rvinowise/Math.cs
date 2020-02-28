using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace rvinowise.rvi {

public class Math
{
    

    public static int sign_or_zero(float value)
    {
        if (System.Math.Abs(value) < float.Epsilon) {
            return 0;
        }
        if (value < 0.0)
            return -1;
        if ( value > 0.0)
            return 1;

        throw new System.ArithmeticException("Arithmetic_NaN");
    }
}

}
