using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;


namespace rvinowise.units
{

public abstract class Tool
{
    public virtual Transform host { get; set; }
    
    //point relative to the host where the leg is attached to it
    public virtual Vector2 attachment{
        get;set;
    }

    public Vector2 position {
        get { return host.TransformPoint(attachment); }
    }
}

}