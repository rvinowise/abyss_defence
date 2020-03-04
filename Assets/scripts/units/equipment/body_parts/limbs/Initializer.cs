using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise;


namespace rvinowise.units.equipment.limbs.init{

public static class Initializer {

    public static void mirror(ILimb2 dst, ILimb2 src) {
        // the base direction is to the right
        dst.attachment = new Vector2(
            src.attachment.x,
            -src.attachment.y
        );
        dst.segment1.possible_span = mirror_span(src.segment1.possible_span);
        //dst.segment1.comfortable_span = mirror_span(src.segment1.comfortable_span);
        dst.segment1.tip = new Vector2(
            src.segment1.tip.x,
            -src.segment1.tip.y
        );

        dst.segment2.possible_span = mirror_span(src.segment2.possible_span);
        //dst.segment2.comfortable_span = mirror_span(src.segment2.comfortable_span);
        dst.segment2.tip = new Vector2(
            src.segment2.tip.x,
            -src.segment2.tip.y
        );
        
        dst.folding_direction = src.folding_direction * -1;

        dst.segment1.spriteRenderer.sprite = src.segment1.spriteRenderer.sprite;
        dst.segment2.spriteRenderer.sprite = src.segment2.spriteRenderer.sprite;
        dst.segment1.spriteRenderer.flipY = !src.segment1.spriteRenderer.flipY;
        dst.segment2.spriteRenderer.flipY = !src.segment2.spriteRenderer.flipY;

        Span mirror_span(Span span) {
            return new Span(
                -span.max,
                -span.min
            );
        }
        
    }

}
}