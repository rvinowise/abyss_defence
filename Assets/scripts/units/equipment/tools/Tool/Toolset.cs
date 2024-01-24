using rvinowise.unity.geometry2d;
using UnityEngine;


namespace rvinowise.unity.units.parts.tools {
public class Toolset: MonoBehaviour {

    public Tool right_tool;
    public Tool left_tool;

    public Tool get_tool_on_side(Side in_side) {
        if (in_side == Side.RIGHT) {
            return right_tool;
        }
        return left_tool;
    }
}




}
