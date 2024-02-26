namespace rvinowise.unity.actions {

public interface IAction_parent: IAction {

void add_child(Action in_child);

void add_children(params Action[] in_children);

}



}