using System;
using System.Collections;
using System.Collections.Generic;
using rvinowise.unity.actions;
using rvinowise.unity.geometry2d;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;


namespace rvinowise.unity {


    
public class Connector_of_parts : MonoBehaviour {

    public Remembering_collision part1_area;
    public Remembering_collision part2_area;

    public void combine_parts() {
        var part1 = get_part(part1_area);
        var part2 = get_part(part2_area);
        if (part1!=null && part2!=null) {
            
        }
    }

    public IAttachable_part get_part(Remembering_collision area) {
        foreach (var part_collider in area.reacheble_colliders) {
            if (part_collider.GetComponent<IAttachable_part>() is {} attachable_part) {
                return attachable_part;
            }
        }
        return null;

    }

}

    
}