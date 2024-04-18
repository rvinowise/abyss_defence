using UnityEngine;
using rvinowise.unity.extensions;
using rvinowise.unity.geometry2d;


namespace rvinowise.unity.actions {

public class Sensor_pay_attention_to_target: Action_leaf {

    private ISensory_organ sensor;
    private Transform target;
    
    public static Sensor_pay_attention_to_target create(
        ISensory_organ sensor,
        Transform in_target
    ) {
        var action = (Sensor_pay_attention_to_target)object_pool.get(typeof(Sensor_pay_attention_to_target));
        action.sensor = sensor;

        action.target = in_target;
        
        return action;
    }

    protected override void on_start_execution() {
        base.on_start_execution();
        
        sensor.pay_attention_to_target(target);
    }

    public override void update() {
        base.update();
        if (sensor.is_focused_on_target()) {
            mark_as_completed();
        }
        else {
            mark_as_not_completed();
        }
        
    }


}
}