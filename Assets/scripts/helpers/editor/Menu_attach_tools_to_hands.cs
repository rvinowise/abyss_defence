using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text.RegularExpressions;


using rvinowise.unity.extensions;
using rvinowise.unity.geometry2d;
using UnityEngine;
using UnityEditor;

using UnityEditor.SceneManagement;


namespace rvinowise.unity {

[InitializeOnLoad]
public static class Menu_attach_tools_to_hands {



	[MenuItem("GameObject/Edit Automatically/Attach tools to hands")]
	private static void attach_tools_to_hands() {
		var hands = Selection.activeGameObject.GetComponentsInChildren<Hand>();
		foreach (var hand in hands) {
			var held_parts = hand.GetComponentsInChildren<Holding_place>();
			if (held_parts.Length > 1) {
				EditorUtility.DisplayDialog(
					"too many held parts",
					$"the hand {hand} has {held_parts.Length} of held parts", "Ok");
			} else if (held_parts.Length == 1) {
				hand.attach_holding_part(held_parts[0]);
			}
		}
		EditorSceneManager.MarkSceneDirty(Selection.activeGameObject.scene);
	}


}


}