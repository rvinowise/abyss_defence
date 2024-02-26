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
public static class Menu_init_segmented_limbs {



	[MenuItem("GameObject/Edit Automatically/Init segmented limbs")]
	private static void init_segmented_limbs() {
		Init_segmented_limbs.init_segmented_limbs(Selection.activeGameObject);
		EditorSceneManager.MarkSceneDirty(Selection.activeGameObject.scene);
	}


}


}