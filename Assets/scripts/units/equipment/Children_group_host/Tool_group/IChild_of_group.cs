using UnityEngine;

namespace rvinowise.unity {

/* "Group" here means a system of tools-children working together (legs, arms etc) */
public interface IChild_of_group
{
    Transform transform{get;}
    //GameObject gameObject{get;}
}

}