using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise;
using UnityEditor;


namespace rvinowise.units.parts.limbs.init{

public static class Initializer {

    public static void mirror(ILimb2 dst, ILimb2 src) {
        // the base direction is to the right
        mirror_segment(dst.segment1, src.segment1);
        mirror_segment(dst.segment2, src.segment2);

        dst.folding_direction = src.folding_direction * -1;
    }
    
    public static void mirror(ILimb3 dst, ILimb3 src) {
        mirror((ILimb2)dst, (ILimb2)src);
        mirror_segment(dst.segment3, src.segment3);
    }

    private static void mirror_segment(Segment dst, Segment src) {
        dst.attachment = new Vector2(
            src.attachment.x,
            -src.attachment.y
        );
        dst.possible_span = mirror_span(src.possible_span);
        dst.tip = new Vector2(
            src.tip.x,
            -src.tip.y
        );
        
        //dst.folding_direction = src.folding_direction * -1;

        dst.spriteRenderer.sprite = src.spriteRenderer.sprite;
        dst.spriteRenderer.flipY = !src.spriteRenderer.flipY;
    }
    
    private static Span mirror_span(Span span) {
        return new Span(
            -span.max,
            -span.min
        );
    }
}
}