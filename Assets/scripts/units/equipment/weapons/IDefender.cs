using rvinowise.unity.actions;
using UnityEngine;


namespace rvinowise.unity {
public interface IDefender: IActing_role
{
    void start_defence(Transform target, System.Action on_completed);
    void finish_defence(System.Action on_completed);
}



}