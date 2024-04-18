using System.Collections.Generic;
using System.Linq;
using rvinowise.unity.debug;
using UnityEngine;


namespace rvinowise.unity
{

/*
represents a coherent system of several objects, 
which work together under control of this object:
creeping_legs, Weapons etc. 
*/
public class Children_group:
    Abstract_children_group 
{

    public List<Child_of_group> children = new List<Child_of_group>();
    public override IEnumerable<IChild_of_group> get_children() => children;
    
    public IList<IChild_of_group> children_stashed_from_copying {
        get;
        private set;
    }

    public override void hide_children_from_copying() {
        base.hide_children_from_copying();
        children.Clear();
    }
    


    public override void add_child(IChild_of_group in_child) {
        children.Add(in_child as Child_of_group);
        in_child.transform.SetParent(this.transform, false);
    }



    
}

}