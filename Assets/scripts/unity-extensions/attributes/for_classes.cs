using System;

namespace rvinowise.unity.extensions.attributes {


[
    AttributeUsage(
        AttributeTargets.Class, 
        AllowMultiple = false
    )
]
public class singleton: System.Attribute {
    
}


}