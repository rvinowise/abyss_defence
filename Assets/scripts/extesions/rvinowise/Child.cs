using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;


namespace rvinowise
{

public interface Child
{
    Transform parent { get; set; }
    
    //point relative to the parent where this Child is attached to it
    Vector2 local_position {
        get;
        set;
    }


    

}

}