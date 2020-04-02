using System.Collections.Generic;
using System.Linq;
using rvinowise.units.debug;
using UnityEngine;
using rvinowise.rvi.contracts;

namespace rvinowise.units.parts
{

/*
represents a coherent system of several objects, 
which work together under control of this object:
creeping_legs, Weapons etc. 
*/
public abstract class Children_group:
    IChildren_group 
    
{
    public IChildren_groups_host host;
    public GameObject game_object {
        get { return host.game_object; }
    }

    public Transform transform {
        get {
            return game_object.transform;
        }
    }
    
    public abstract IEnumerable<Child> children {
        get;
    }

    public bool has_child(Child in_child) {
        return children.Any(tool => tool == in_child);
    }
    
    
    protected Children_group(GameObject in_game_object, IChildren_groups_host in_host) {
        debug = new Debug(this);
        debug.increase_counter();

        host = in_host;
        host.children_groups.Add(this);
        
        init_components();
    }
    
    protected Children_group(IChildren_groups_host in_host) {
        debug = new Debug(this);
        debug.increase_counter();
        
        //game_object = in_game_object;
        host = in_host;
        host.children_groups.Add(this);
        
        init_components();
    }

    protected Children_group() {
        debug = new Debug(this);
        debug.increase_counter();
        
        init_components();
    }
    
    protected virtual void init_components() { }
    

    ~Children_group() {
        debug.decrease_counter();
    }
    
    public void add_to_user(IChildren_groups_host in_user) {
        host = in_user;
    }


    public abstract void add_child(Child child);

    public virtual void init() { }



    public virtual void distribute_data_across(
        IEnumerable<Children_group> new_controllers
    ) {}

    

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