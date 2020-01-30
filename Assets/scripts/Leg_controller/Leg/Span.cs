using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace units {
namespace limbs {

public class Span {
    /* possible rotation of a segment relative to it's attachment  */
    public float min; //maximum rotation to the left (counter-clockwise)
    public float max; //maximum rotation to the right (clockwise)
    public Span(float _min, float _max) {
        min = _min;
        max = _max;
    }
}

}
}