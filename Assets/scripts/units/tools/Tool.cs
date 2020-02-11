using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;


namespace units
{

public abstract class Tool
{
    public Transform body;
    //point relative to the body where the leg is attached to it
    public virtual Vector2 attachment{
        get;set;
    }
}

}