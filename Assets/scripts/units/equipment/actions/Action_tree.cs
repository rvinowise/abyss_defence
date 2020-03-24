using System.Collections.Generic;

namespace rvinowise.units.parts.actions {
public class Action_tree {

    public IList<Action> strategies;
    //private IDo_actions user;

    private Microsoft.Extensions.ObjectPool.ObjectPool<parts.actions.Action> pool;
    

    public Action current_action {
        set {
            change(value);
        }
        get { return _current_action; }
    }
    private Action _current_action;

    public Action next {
        get { return _last_added_action.next; }
        set {
            _last_added_action.next = value;
            _last_added_action = value;
        }
    }
    private Action _last_added_action;

    public Action_tree() {
        //this.user = user;
    }

    public void change(Action new_action) {
        _current_action?.end();
        _current_action = new_action;
        _last_added_action = _current_action;
        _current_action.start();
    }
    
    public void update() {
        current_action.update();
    }
    
    public void deactivate() {
        //current_action
    }

    
    

}
}