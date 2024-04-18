using rvinowise.unity.extensions;
using rvinowise.unity.geometry2d;
using rvinowise.unity.actions;
using UnityEngine;

namespace rvinowise.unity {

public class Init_segmented_limbs {
    private static void init_segment_lengths(Segment segment) {
        Segment next_segment = segment.GetComponentInDirectChildren<Segment>();
        Transform tip_tramsform = segment.transform.Find("tip");
		
        if ((next_segment == null) == (tip_tramsform == null))
            Debug.LogError(
                $"tip of a segment should be assigned either by next segment of by the tip-transform," +
                $"skipping {segment.name}"
            );
		
        if (next_segment!=null) {
            var position = next_segment.transform.localPosition * segment.transform.lossyScale.x;
            position.y = 0;
            segment.localTip  = position;
        } else if (tip_tramsform != null) {
            var position = tip_tramsform.localPosition * segment.transform.lossyScale.x;
            position.y = 0;
            segment.localTip = position;
        }
    }
	
    private static void init_folding_direction(Limb2 limb) {
        limb.folding_side = Side.mirror(limb.segment2.possible_span.side_of_bigger_rotation());
        rvinowise.contracts.Contract.Ensures(
            limb.folding_side != Side_type.NONE,
            "rotation span of Segment #2 should define folding direction of the limb"
        );
    }

    public static void init_folding_directions_of_limbs(GameObject limbs_owner) {
        var limbs = limbs_owner.GetComponentsInChildren<Limb2>();
        foreach (var limb in limbs) {
            init_folding_direction(limb);
        }
    }

    public static void init_lengths_of_segments(GameObject segment_owner) {
        var segments = segment_owner.GetComponentsInChildren<Segment>();
        foreach (var segment in segments) {
            init_segment_lengths(segment);
            if (segment.transform.parent) {
                segment.parent_segment = segment.transform.parent.GetComponent<Segment>();
            }
        }
    }
    
    public static void init_segmented_limbs(GameObject limbs_owner) {
        init_lengths_of_segments(limbs_owner);
        init_folding_directions_of_limbs(limbs_owner);
    }
}

}