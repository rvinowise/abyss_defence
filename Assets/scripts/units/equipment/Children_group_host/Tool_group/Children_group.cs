using System.Collections.Generic;
using System.Linq;
using rvinowise.unity.debug;
using UnityEngine;
using rvinowise.unity.extensions;
using rvinowise.contracts;

namespace rvinowise.unity.units.parts
{

/*
represents a coherent system of several objects, 
which work together under control of this object:
creeping_legs, Weapons etc. 
*/
public abstract class Children_group:
    MonoBehaviour,
    IChildren_group 
    
{
    public IChildren_groups_host host;

    public abstract IEnumerable<ICompound_object> children {
        get;
    }

    public IEnumerable<ICompound_object> children_stashed_from_copying {
        get;
        protected set;
    }

    public GameObject game_object {
        get { return this.gameObject; }
    }

    public Transform transform {
        get {
            return game_object.transform;
        }
    }

    public bool has_child(ICompound_object in_compound_object) {
        return children.Any(tool => tool == in_compound_object);
    }
      
    public abstract void hide_children_from_copying();
    
    protected virtual void Awake() {
        host = GetComponent<IChildren_groups_host>();
        if (host != null) {
            host.children_groups.Add(this);
        }

        debug = new Debug(this);
        debug.increase_counter();

        init_components();
    }

    protected virtual void Start() {
        
    }
    

    
    protected virtual void init_components() { }
    

    ~Children_group() {
        debug.decrease_counter();
    }



    public abstract void add_child(ICompound_object compound_object);

    
    public virtual void init() { }



    public virtual void distribute_data_across(
        IEnumerable<Children_group> new_controllers
    ) {}

    public virtual void shift_center(Vector2 in_shift) {
        foreach (ICompound_object child in children) {
            child.main_object.transform.localPosition += (Vector3)in_shift;
        }
    }
    

    public class Debug: Debugger {
        protected override ref int count {
            get { return ref _count; }
        }
        static protected int _count = 0;

        public Debug(Children_group in_controller):base(in_controller) {
        }
    }

    public Debug debug;

    

    

    public virtual void on_draw_gizmos() {
    
    }
    
}

}