using System.Collections.Generic;
using System.Linq;
using rvinowise.unity.actions;
using UnityEngine;


namespace rvinowise.unity {

public class Compound_sensory_organ:
    Abstract_children_group,
    ISensory_organ
{

    //public List<GameObject> sensor_objects = new List<GameObject>(); // for inspector
    public List<Sensor_child_of_group> child_sensors;

    public static Compound_sensory_organ create(
        Intelligence host,
        IList<ISensory_organ> sensors
    ) {
        var tool_object = new GameObject("Compound_sensory_organ");
        var tool_component = tool_object.AddComponent<Compound_sensory_organ>();
        tool_component.child_sensors = 
            sensors
                .Select(tool => tool as Sensor_child_of_group)
                .Where(tool => tool != null)
                .ToList();
        tool_object.transform.parent = host.transform;
        return tool_component;
    }
    
    
    
    public Compound_sensory_organ(IEnumerable<Sensor_child_of_group> in_child_sensors) {
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

    #region Children_group

    public override IEnumerable<Child_of_group> get_children() {
        return child_sensors;
    }

    public override void add_child(Child_of_group in_child) {
        if (in_child is Sensor_child_of_group sensor) {
            child_sensors.Add(sensor);
        }
    }

    public override void hide_children_from_copying() {
        base.hide_children_from_copying();
        child_sensors.Clear();
    }

    #endregion
    

    public Actor actor { get; set; }

    public void on_lacking_action() {
        Idle.create(this).start_as_root(actor.action_runner);
    }
    
    

}
}