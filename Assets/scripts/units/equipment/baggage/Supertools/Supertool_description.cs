using rvinowise.unity.actions;
using UnityEngine;
using rvinowise.unity.geometry2d;
using UnityEngine.Serialization;
using UnityEngine.UI;


namespace rvinowise.unity {

public abstract class Supertool_description: MonoBehaviour {

    public Tool tool;
    public string tool_name;
    public Image image;


    public abstract void start_using_action(Humanoid humanoid);

}
}