using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text.RegularExpressions;


using rvinowise.unity.extensions;
using rvinowise.unity.geometry2d;
using rvinowise.unity.helpers.graphics;
using rvinowise.unity.units.parts;
using rvinowise.unity.units.parts.limbs;
using rvinowise.unity.units.parts.limbs.arms;
using rvinowise.unity.units.parts.limbs.creeping_legs;
using rvinowise.unity.units.parts.teeth;
using UnityEngine;
using UnityEditor;

using UnityEditor.SceneManagement;
using Segment = rvinowise.unity.units.parts.limbs.Segment;


[InitializeOnLoad]
internal static class Init_segmented_limbs {


	private static void init_segment_lengths(Segment segment) {
		Segment next_segment = segment.GetComponentInDirectChildren<Segment>();
		Transform tip_tramsform = segment.transform.Find("tip");
		
		if ((next_segment == null) == (tip_tramsform == null))
			Debug.LogError(
				$"tip of a segment should be assigned either by next segment of by the tip-transform," +
				$"skipping {segment.name}"
			);
		
		if (next_segment!=null) {
			var position = next_segment.transform.localPosition;
			position.y = 0;
			segment.localTip = next_segment.transform.localPosition = position;
		} else if (tip_tramsform != null) {
			var position = tip_tramsform.localPosition;
			position.y = 0;
			segment.localTip = tip_tramsform.localPosition = position;
		}
	}
	
	private static void init_folding_direction(Limb2 limb) {
		limb.folding_side = Side.mirror(limb.segment2.possible_span.side_of_bigger_rotation());
		rvinowise.contracts.Contract.Ensures(
			limb.folding_side != Side_type.NONE,
			"rotation span of Segment #2 should define folding direction of the limb"
		);
	}
	
	private static void init_folding_directions_of_limbs() {
		var limbs = Selection.activeGameObject.GetComponentsInChildren<Limb2>();
		foreach (var limb in limbs) {
			init_folding_direction(limb);
		}
	}
	
	private static void init_lengths_of_segments() {
		var segments = Selection.activeGameObject.GetComponentsInChildren<Segment>();
		foreach (var segment in segments) {
			init_segment_lengths(segment);
			if (segment.transform.parent) {
				segment.parent_segment = segment.transform.parent.GetComponent<Segment>();
			}
		}
	}
		
	
	[MenuItem("GameObject/Edit Automatically/Init segmented limbs")]
	private static void mirror_children() {
		init_lengths_of_segments();
		init_folding_directions_of_limbs();
		EditorSceneManager.MarkSceneDirty(Selection.activeGameObject.scene);
	}


}
 