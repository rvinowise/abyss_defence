using System;

namespace rvinowise.unity.extensions.attributes {


/* in Unity the difference between Prefabs and their Instances is the same
 as the difference between Classes and their Instances, but it's not marked
 by Unity. Static functions are badly integrated into Unity, instead, 
 functions of the Prefab should be used. this attribute marks them 
 exlicitly
 */
[
    AttributeUsage(
        AttributeTargets.Method, 
        AllowMultiple = false
    )
]
public class called_by_prefab: System.Attribute {
    
}


/* Animator invokes this method from a keyframe */
[
    AttributeUsage(
        AttributeTargets.Method, 
        AllowMultiple = false
    )
]
public class called_in_animation: System.Attribute {
    
}
}