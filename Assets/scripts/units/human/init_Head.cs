using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise;
using rvinowise.units.parts.limbs;


namespace rvinowise.units.humanoid.init {

public class Head {

    public static parts.head.Head init(
        Humanoid parent,
        parts.head.Head head) 
    {
        head.possible_span = new Span(90,-90);
        head.rotation_speed = 340;
        head.parent = parent.transform;
        head.local_position = new Vector2(0,0);
        head.spriteRenderer.sprite = parent.head_sprite;
        head.spriteRenderer.sortingLayerName = "on body";
        head.transform.parent = parent.transform;
        head.transform.localPosition = head.local_position;
        return head;
    }

}
}