using System.Collections.Generic;
using System.Linq;
using rvinowise.unity.debug;
using UnityEngine;


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

    public Divisible_body divisible_body;

    public abstract IEnumerable<IChild_of_group> children {
        get;
    }

    public IEnumerable<IChild_of_group> children_stashed_from_copying {
        get;
        protected set;
    }

    public void hide_children_from_copying() {
        children_stashed_from_copying = children.Where(leg => leg != null) as IEnumerable<IChild_of_group> ;
        init_child_list();
    }
    protected abstract void init_child_list();
    


    public bool has_child(IChild_of_group in_compound_object) {
        return children.Any(tool => tool == in_compound_object);
    }
      
    
    protected virtual void Awake() {
        host = GetComponent<IChildren_groups_host>();
        if (host != null) {
            host.children_groups.Add(this);
        }

        if (divisible_body == null) {
            divisible_body = GetComponent<Divisible_body>();
        }

        debug = new Debug(this);
        debug.increase_counter();
    }

    protected virtual void Start() {
        
    }

    
    

    ~Children_group() {
        debug.decrease_counter();
    }



    public abstract void add_child(IChild_of_group in_child);

    
    public virtual void init() { }



    public virtual void distribute_data_across(
        IEnumerable<Children_group> new_controllers
    ) {}

    public virtual void shift_center(Vector2 in_shift) {
        foreach (IChild_of_group child in children) {
            child.transform.localPosition += (Vector3)in_shift;
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