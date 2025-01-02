


using UnityEngine;

namespace rvinowise.unity.actions {

public interface IActing_role {
    Actor actor {get; set; }
    void on_lacking_action();
}
}