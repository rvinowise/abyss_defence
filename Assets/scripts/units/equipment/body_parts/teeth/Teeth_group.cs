using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using rvinowise.unity.extensions;

using rvinowise.contracts;
using rvinowise.unity.geometry2d;
using rvinowise.unity.units.parts.transport;
using UnityEngine.Assertions;
using static rvinowise.unity.geometry2d.Directions;

namespace rvinowise.unity.units.parts.teeth {
public class Teeth_group :
Children_group
{
    public override IEnumerable<IChild_of_group> children => teeth;
    public List<Tooth> teeth;

    public override void add_child(IChild_of_group in_child)
    {
        Tooth tooth = in_child as Tooth;
        teeth.Add(tooth);
        tooth.transform.SetParent(transform, false);
    }

    protected override void init_child_list() {
        teeth = new List<Tooth>();
    }
}

}