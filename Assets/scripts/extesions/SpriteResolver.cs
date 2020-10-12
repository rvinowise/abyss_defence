using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using extensions.pooling;
using rvinowise.rvi.contracts;
using UnityEngine;
using UnityEngine.Experimental.U2D.Animation;

public static partial class Unity_extension
{

   

    public static int get_label_as_number(
        this SpriteResolver in_resolver
    ) {
        return Int32.Parse(in_resolver.GetLabel());

    }
    
}
