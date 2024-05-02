


using UnityEngine;

namespace rvinowise.unity.actions {

public interface IActor: IRunning_actions
{
    Action current_action { set; get; }
    void on_lacking_action();
}
}