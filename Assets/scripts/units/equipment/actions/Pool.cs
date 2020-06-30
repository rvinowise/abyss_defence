using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise;



namespace rvinowise.units.parts.actions {

public abstract partial class Action {
    
    public class Pool<TBase> where TBase : units.parts.actions.Action {
        private Dictionary<System.Type, Queue<TBase>> types_to_objects = 
            new Dictionary<Type, Queue<TBase>>();

        public TBase get(
            System.Type type, 
            Action_parent in_action_parent
        ) {
            Queue<TBase> bases = get_or_create_place_for_type(type);

            if (bases.Count == 0) {
                TBase new_base = create_new_object(type, in_action_parent);
                return new_base;
            }
            TBase restored_action = bases.Dequeue();
            restored_action.attach_to_parent(in_action_parent);
            
            return restored_action;
        }

        public void return_to_pool(TBase obj) {
            Queue<TBase> bases = get_or_create_place_for_type(obj.GetType());
            bases.Enqueue(obj);
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
            Type type, 
            Action_parent in_parent
        ) {
            TBase new_base = (TBase) Activator.CreateInstance(type);
            new_base.parent_action = in_parent;
            return new_base;
        }
    }
}

}