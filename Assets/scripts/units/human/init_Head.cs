using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise;
using rvinowise.units.parts.limbs;


namespace rvinowise.units.human.init {

public class Head {

    public static parts.head.Head init(
        Human host,
        parts.head.Head head) 
    {
        head.possible_span = new Span(90,-90);
        head.rotation_speed = 10;
        head.host = host.transform;
        head.attachment = new Vector2(0,0);
        head.spriteRenderer.sprite = host.head_sprite;
        head.spriteRenderer.sortingLayerName = "on body";
        return head;
    }

}
}