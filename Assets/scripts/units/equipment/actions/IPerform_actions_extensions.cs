


namespace rvinowise.unity.actions {

public static class IPerform_actions_extensions {

    public static Action get_root_action(this Actor actor) {
        if (actor.current_action != null) {
            return actor.current_action.get_root_action();
        }
        return null;
    }

}
}