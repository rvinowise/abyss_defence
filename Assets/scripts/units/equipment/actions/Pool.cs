using System;
using System.Collections.Generic;
using Debug = rvinowise.unity.debug.Debug;

namespace rvinowise.unity.actions {

public abstract partial class Action {
    
    public class Pool<TBase> where TBase : Action {
        private Dictionary<System.Type, Queue<TBase>> types_to_objects = 
            new Dictionary<Type, Queue<TBase>>();


        public TBase get(System.Type type) {
            Queue<TBase> bases = get_or_create_place_for_type(type);

            if (bases.Count == 0) {
                TBase new_base = create_new_object(type);
                return new_base;
            }
            TBase restored_action = bases.Dequeue();
            restored_action.is_free_in_pool = false;
            restored_action.id = Guid.NewGuid();

            return restored_action;
        }

        private bool check_if_correctly_cleaned(Action in_action) {
            if (in_action is Action_sequential_parent sequential_parent) {
                Debug.Assert(sequential_parent.current_child_action == null);
                Debug.Assert(sequential_parent.queued_child_actions.Count == 0);
            } else if (in_action is Action_parallel_parent parallel_parent) {
                Debug.Assert(parallel_parent.child_actions.Count == 0);
            }
            Debug.Assert(in_action.parent_action == null);
            Debug.Assert(!in_action.is_completed);
            return true;
        }

        public void return_to_pool(TBase obj) {
            check_if_correctly_cleaned(obj);
            
            Queue<TBase> bases = get_or_create_place_for_type(obj.GetType());
            bases.Enqueue(obj);
            obj.is_free_in_pool = true;
        }

        private Queue<TBase> get_or_create_place_for_type(System.Type type) {
            Queue<TBase> bases;
            types_to_objects.TryGetValue(type, out bases);
            if (bases == null) {
                bases = new Queue<TBase>();
                types_to_objects.Add(type, bases);
            }
            return bases;
        }


        TBase create_new_object(
            Type type
        ) {
            TBase new_base = (TBase) Activator.CreateInstance(type);
            return new_base;
        }
    }
}

}