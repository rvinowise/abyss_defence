using rvinowise.unity.actions;
using UnityEngine;
using Action = System.Action;


namespace rvinowise.unity {
public interface IAttacker: IActing_role
{
    bool is_weapon_ready_for_target(Transform target);
    float get_reaching_distance();
    void attack(Transform target, System.Action on_completed = null);
}




public abstract class Attacker_child_of_group: 
    Child_of_group,
    IAttacker 
{

    public abstract bool is_weapon_ready_for_target(Transform target);
    public abstract float get_reaching_distance();

    public abstract void attack(Transform target, Action on_completed = null);

    public virtual void on_lacking_action() {}

    public Actor actor { get; set; }

}

}