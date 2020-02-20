using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace rvinowise.units {

public interface IControl {
    float vertical {get;}
    float horizontal {get;}
    float rotation {get;}

    void read_input();
}

}