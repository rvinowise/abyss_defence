using rvinowise.unity.geometry2d;
using UnityEngine;


namespace rvinowise.unity {
public class Toolset: MonoBehaviour {

    public Tool right_tool;
    public Tool left_tool;

    public Tool get_tool_on_side(Side_type in_side) {
        if (in_side == Side_type.RIGHT) {
            return right_tool;
        }
        return left_tool;
    }
}




}
