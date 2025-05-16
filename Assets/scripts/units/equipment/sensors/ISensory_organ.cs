using rvinowise.unity.actions;
using UnityEngine;


namespace rvinowise.unity {

public interface ISensory_organ: IActing_role {

    void pay_attention_to_target(Transform target);

    bool is_focused_on_target();
}

public abstract class Sensor_child_of_group: 
    Child_of_group,
    ISensory_organ
{

    public abstract void pay_attention_to_target(Transform target);

    public abstract bool is_focused_on_target();
    public virtual void on_lacking_action() { }
    public Actor actor { get; set; }


}

}