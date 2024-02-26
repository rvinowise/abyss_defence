

namespace rvinowise.unity {
public interface IBaggae_change_listener
{
    void update_ammo(Tool in_tool, int change);
    void update_available_tools(Tool in_tool, int change);
}

}