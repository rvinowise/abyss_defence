using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using rvinowise.unity;
using rvinowise.unity.extensions;
using rvinowise.unity.geometry2d;
using UnityEngine;
using UnityEditor;

using UnityEditor.SceneManagement;


[InitializeOnLoad]
internal static class Mirroring_prefab_children {


	private static void rename_for_opposite_side(GameObject dst) {
		IDictionary<string,string> name_parts = new Dictionary<string, string>
		{
			{"_l$","_r"},
			{"_r$","_l"},
			{"_l_","_r_"},
			{"_r_","_l_"},
			{"(clone)",""},
			{"(Clone)",""},
		};
		var regex = new Regex(String.Join("|",name_parts.Keys.Select(Regex.Escape)));
		dst.name = regex.Replace(dst.name, m => name_parts[m.Value]);
	}
    
	private static void mirror_segment(Segment src, Segment dst) {
		dst.transform.localPosition = new Vector2(
			src.transform.localPosition.x,
			-src.transform.localPosition.y
		);
		dst.transform.localRotation =
			src.transform.localRotation.inverse();
			
        
		dst.possible_span = src.possible_span.mirror().init_for_direction(-src.local_degrees);

		dst.sprite_renderer = dst.GetComponent<SpriteRenderer>();
		if (dst.sprite_renderer != null) {
			dst.sprite_renderer.sprite = src.sprite_renderer.sprite;
			dst.sprite_renderer.flipY = !src.sprite_renderer.flipY;
		}
		
	}
	
	
	private static void mirror_arm_segment(
		rvinowise.unity.Arm_segment src,
		rvinowise.unity.Arm_segment dst
	) {
		mirror_segment(src,dst);
		dst.desired_idle_rotation = Quaternion.Inverse(src.desired_idle_rotation);
	}
	
	public static void mirror_hand_segment(Hand src, Hand dst) {
		mirror_segment(src,dst);
		dst.side = Side.mirror(src.side);
	}
	
	private static Arm create_mirrored_arm(Arm src) {
		Arm dst = GameObject.Instantiate(src).GetComponent<Arm>();
		mirror_arm_segment(src.forearm,dst.forearm);
		mirror_arm_segment(src.upper_arm,dst.upper_arm);
		mirror_hand_segment(src.hand, dst.hand);
		rename_for_opposite_side(dst.gameObject);
		return dst;
	}
	
	private static Leg2 create_mirrored_leg2(Leg2 src)
	{
		Leg2 dst = GameObject.Instantiate(src).GetComponent<Leg2>();
		dst.local_position = new Vector2(
			src.local_position.x,
			-src.local_position.y
		);
		mirror_segment(src.femur, dst.femur);
		mirror_segment(src.tibia, dst.tibia);

		dst.optimal_relative_position_standing_transform.localPosition =
			src.optimal_relative_position_standing_transform.localPosition.mirror();

		rename_for_opposite_side(dst.gameObject);
		return dst;
	}

	private static Leg3 create_mirrored_leg3(Leg3 src)
	{
		Leg3 dst = GameObject.Instantiate(src).GetComponent<Leg3>();
		dst.local_position = new Vector2(
			src.local_position.x,
			-src.local_position.y
		);

		mirror_segment(src.coxa, dst.coxa);
		mirror_segment(src.femur, dst.femur);
		mirror_segment(src.tibia, dst.tibia);

		dst.optimal_relative_position_standing_transform.localPosition =
			src.optimal_relative_position_standing_transform.localPosition.mirror();

		rename_for_opposite_side(dst.gameObject);
		return dst;
	}
	
	private static Tooth create_mirrored_tooth(Tooth src)
	{
		Tooth dst = GameObject.Instantiate(src).GetComponent<Tooth>();
		// the base direction_quaternion is to the right
		dst.transform.localPosition = 
			src.transform.localPosition.mirror();
		dst.transform.localRotation = src.transform.localRotation.inverse();
      
		dst.sprite_renderer.flipY = !src.sprite_renderer.flipY;

		rename_for_opposite_side(dst.gameObject);
		return dst;
	}
	
	
    
	private static void duplicate_mirrored_children(Children_group children_group) {
		IList<IChild_of_group> initial_children = new List<IChild_of_group>(children_group.children);
		foreach(var src_child in initial_children) {
			IChild_of_group dst_child = src_child switch {
				Leg2 src_leg2 => create_mirrored_leg2(src_leg2),
				Arm src_arm => create_mirrored_arm(src_arm),
				Leg3 src_leg3 => create_mirrored_leg3(src_leg3),
				Tooth src_tooth => create_mirrored_tooth(src_tooth),
				_ => src_child
			};
			children_group.add_child(dst_child);
		}
	}
	
    
	[MenuItem("GameObject/Edit Automatically/Mirror Children")]
	private static void mirror_children() {
		var children_group = Selection.activeGameObject.GetComponent<Children_group>();
		duplicate_mirrored_children(children_group);
		EditorSceneManager.MarkSceneDirty(Selection.activeGameObject.scene);
	}


}
 