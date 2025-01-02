using rvinowise.unity.actions;
using UnityEngine;


namespace rvinowise.unity {

public interface ISensory_organ: IActing_role {

    void pay_attention_to_target(Transform target);

    bool is_focused_on_target();
}



}