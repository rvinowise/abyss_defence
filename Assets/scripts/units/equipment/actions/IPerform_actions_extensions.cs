


namespace rvinowise.unity.units.parts.actions {

public static class IPerform_actions_extensions {

    public static Action get_root_action(this IPerform_actions actor) {
        if (actor.current_action != null) {
            return actor.current_action.get_root_action();
        }
        return null;
    }

}
}