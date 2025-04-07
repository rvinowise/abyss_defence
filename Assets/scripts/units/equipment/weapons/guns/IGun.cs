using UnityEngine;


namespace rvinowise.unity {

public interface IGun:IChild_of_group {

    public enum Vertical_pointing {
        GROUND, AIR 
    }
    
    void pull_trigger();
    void release_trigger();

    bool can_fire();
    bool is_ready_for_target(Transform target);

    bool is_aiming_automatically();
    void set_vertical_pointing(Vertical_pointing pointing);

    Reloadable get_reloadable();
}
}