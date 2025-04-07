using System.Collections.Generic;
using System.Linq;
using rvinowise.unity.actions;
using UnityEngine;


namespace rvinowise.unity {

public class Compound_sensory_organ:
    MonoBehaviour,
    ISensory_organ {

    public List<GameObject> sensor_objects = new List<GameObject>(); // for inspector
    public IList<ISensory_organ> child_sensors;

    public static Compound_sensory_organ create(
        Intelligence host,
        IList<ISensory_organ> sensors
    ) {
        var tool_object = new GameObject("Compound_sensory_organ");
        var tool_component = tool_object.AddComponent<Compound_sensory_organ>();
        tool_component.child_sensors = sensors.ToList();
        //host.action_runner.add_actor(tool_object.AddComponent<Actor>());
        tool_object.transform.parent = host.transform;
        return tool_component;
    }
    
    protected void Awake() {
        child_sensors = new List<ISensory_organ>();
        foreach (var sensor_object in sensor_objects) {
            if (sensor_object.GetComponent<ISensory_organ>() is { } sensor) {
                child_sensors.Add(sensor);
            }
        }
    }

    public void init_sensors() {
        
    }
    
    
    public Compound_sensory_organ(IEnumerable<ISensory_organ> in_child_sensors) {
        child_sensors = in_child_sensors.ToList();
    }
    
    public void pay_attention_to_target(Transform target) {
        foreach (var child_sensor in child_sensors) {
            child_sensor.pay_attention_to_target(target);
        }
    }

    public bool is_focused_on_target() {
        foreach (var child_sensor in child_sensors) {
            if (child_sensor.is_focused_on_target()) {
                return true;
            }
        }
        return false;
    }


    public Actor actor { get; set; }

    public void on_lacking_action() {
        Idle.create(this).start_as_root(actor.action_runner);
    }

}
}