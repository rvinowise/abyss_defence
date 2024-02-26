using UnityEngine;


namespace rvinowise.unity {
public interface IAttacker: 
    IExecute_commands
    
{
    bool can_reach(Transform target);
    void attack(Transform target, System.Action on_completed = null);
}

public interface IAttacker_child_of_group: 
    IAttacker
    ,IChild_of_group
    
{
}

}