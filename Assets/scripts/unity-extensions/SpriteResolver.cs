using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using rvinowise.unity.extensions.pooling;
using rvinowise.contracts;
using UnityEngine;
using rvinowise.unity.extensions;




namespace rvinowise.unity.extensions {
public static partial class Unity_extension
{

   

    public static int get_label_as_number(
        this UnityEngine.Experimental.U2D.Animation.SpriteResolver in_resolver
    ) {
        return Int32.Parse(in_resolver.GetLabel());

    }
    
}
}
