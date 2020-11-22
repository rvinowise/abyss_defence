


namespace rvinowise.unity.units.parts.actions {

public interface IPerform_actions {

    Action current_action { set; get; }

    void set_root_action(Action in_root_action);

}
}