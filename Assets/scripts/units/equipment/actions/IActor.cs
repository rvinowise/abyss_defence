


using UnityEngine;

namespace rvinowise.unity.units.parts.actions {

public interface IActor {

    Action current_action { set; get; }

    void on_lacking_action();
    
    GameObject gameObject { get; }
}
}