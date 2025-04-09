using System.Collections.Generic;
using rvinowise.contracts;
using rvinowise.unity.actions;
using UnityEngine;


namespace rvinowise.unity
{
public class Attachment_points_group: 
    Abstract_children_group
{

    public List<Attachment_point> attachment_points = new List<Attachment_point>();
    public override IEnumerable<IChild_of_group> get_children() {
        return attachment_points;
    }


    protected void Awake() {
        foreach (var attachment_point in GetComponentsInChildren<Attachment_point>()) {
            attachment_points.Add(attachment_point);
        }
    }
    
    
    public override void hide_children_from_copying() {
        base.hide_children_from_copying();
        attachment_points.Clear();
    }
    public override void add_child(IChild_of_group in_child) {
        Contract.Requires(in_child is Attachment_point);
        if (in_child is Attachment_point attachment_point)
        {
            attachment_points.Add(attachment_point);
            in_child.transform.SetParent(transform, false);
        }
    }

}

}

