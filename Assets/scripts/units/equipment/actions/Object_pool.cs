using System;
using System.Collections.Generic;
using Debug = rvinowise.unity.debug.Debug;

namespace rvinowise.unity.actions {

    
public class Object_pool<TBase> where TBase : Action {
    private readonly Dictionary<Type, Stack<TBase>> types_to_objects = 
        new Dictionary<Type, Stack<TBase>>();


    public TBase get(Type type) {
        Stack<TBase> bases = get_or_create_place_for_type(type);

        if (bases.Count == 0) {
            TBase new_base = create_new_object(type);
            return new_base;
        }
        TBase restored_action = bases.Pop();
        restored_action.is_free_in_pool = false;
        restored_action.id = Guid.NewGuid();

        return restored_action;
    }
    
  
    
    public TChild get<TChild>() where TChild: TBase {
        Stack<TBase> bases = get_or_create_place_for_type<TChild>();

        if (bases.Count == 0) {
            TBase new_base = create_new_object<TChild>();
            return (TChild)new_base;
        }
        TBase restored_action = bases.Pop();
        restored_action.is_free_in_pool = false;
        restored_action.id = Guid.NewGuid();

        return (TChild)restored_action;
    }

    private void check_if_correctly_cleaned(Action in_action) {
        if (in_action is Action_sequential_parent sequential_parent) {
            Debug.Assert(sequential_parent.current_child_action == null);
            Debug.Assert(sequential_parent.queued_child_actions.Count == 0);
        } else if (in_action is Action_parallel_parent parallel_parent) {
            Debug.Assert(parallel_parent.child_actions.Count == 0);
        }
        Debug.Assert(in_action.parent_action == null);
        Debug.Assert(!in_action.is_completed);
    }

    public void return_to_pool(TBase obj) {
        check_if_correctly_cleaned(obj);
        
        Stack<TBase> bases = get_or_create_place_for_type(obj.GetType());
        bases.Push(obj);
        obj.is_free_in_pool = true;
    }

    private Stack<TBase> get_or_create_place_for_type(Type type) {
        Stack<TBase> bases;
        types_to_objects.TryGetValue(type, out bases);
        if (bases == null) {
            bases = new Stack<TBase>();
            types_to_objects.Add(type, bases);
        }
        return bases;
    }
    
    private Stack<TBase> get_or_create_place_for_type<TChild>() {
        Stack<TBase> bases;
        types_to_objects.TryGetValue(typeof(TChild), out bases);
        if (bases == null) {
            bases = new Stack<TBase>();
            types_to_objects.Add(typeof(TChild), bases);
        }
        return bases;
    }


    TBase create_new_object(
        Type type
    ) {
        TBase new_base = (TBase) Activator.CreateInstance(type);
        return new_base;
    }
    
    TBase create_new_object<TChild>() {
        TBase new_base = (TBase) Activator.CreateInstance(typeof(TChild));
        return new_base;
    }
}
}

