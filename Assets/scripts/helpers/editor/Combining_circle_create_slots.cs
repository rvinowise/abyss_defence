using System;
using System.Linq;
using rvinowise.unity;
using rvinowise.unity.extensions;
using rvinowise.unity.geometry2d;
using UnityEngine;
using UnityEditor;

using UnityEditor.SceneManagement;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;


[InitializeOnLoad]
public class Combining_circle_create_slots: EditorWindow {


	
	System.Random random = new System.Random();
	public float rotationAmount;
	public string selected = "";

	[MenuItem("GameObject/Edit Automatically/Combining circle/create slots")]
	static void open_dialog()
	{
		Combining_circle_create_slots window =
			EditorWindow.GetWindow<Combining_circle_create_slots>(true, "Randomize Objects");
		window.ShowModalUtility();
	}

	private TextField number_edit;
	void CreateGUI()
	{
		var label = new Label("Specify number of slots");
		rootVisualElement.Add(label);

		var add_for_all_rings = new Button {
			text = "add slots to all rings"
		};
		add_for_all_rings.clicked += create_slots_for_all_circles;
		rootVisualElement.Add(add_for_all_rings);
		
		var add_for_selected_ring = new Button {
			text = "add slots to selected ring"
		};
		add_for_selected_ring.clicked += create_slots_for_selected_circle;
		rootVisualElement.Add(add_for_selected_ring);
		
		var add_children_along_circle = new Button {
			text = "add copies of selected child along a circle"
		};
		add_children_along_circle.clicked += create_children_along_a_circle_for_selected_child;
		rootVisualElement.Add(add_children_along_circle);

		number_edit = new UnityEngine.UIElements.TextField();
		rootVisualElement.Add(number_edit);
		
		var update_slot_position = new Button {
			text = "update slot position for selected ring"
		};
		update_slot_position.clicked += move_slots_to_their_ring_radius_for_selected_ring;
		rootVisualElement.Add(update_slot_position);
		
		var update_slot_position_all_rings = new Button {
			text = "update slot position for all rings"
		};
		update_slot_position_all_rings.clicked += move_slots_to_their_ring_radius_for_all_rings;
		rootVisualElement.Add(update_slot_position_all_rings);
	}

	
	private void create_slots_for_all_circles() {
		var slots_number = Int32.Parse(number_edit.value);
		var rings = Selection.activeGameObject.GetComponentsInChildren<Combining_circle_ring>();
        foreach(var ring in rings) {
	        add_unit_slots(ring, slots_number);
        }
		EditorSceneManager.MarkSceneDirty(Selection.activeGameObject.scene);
	}
	
	private void create_slots_for_selected_circle() {
		var slots_number = Int32.Parse(number_edit.value);
		var ring = Selection.activeGameObject.GetComponent<Combining_circle_ring>();
		add_unit_slots(ring, slots_number);
		EditorSceneManager.MarkSceneDirty(Selection.activeGameObject.scene);
	}
	
	private void move_slots_to_their_ring_radius_for_all_rings() {
		var rings = Selection.activeGameObject.GetComponentsInChildren<Combining_circle_ring>();
		foreach(var ring in rings) {
			move_slots_to_their_ring_radius(ring.transform);
		}
		EditorSceneManager.MarkSceneDirty(Selection.activeGameObject.scene);
	}
	
	private void move_slots_to_their_ring_radius_for_selected_ring() {
		var ring = Selection.activeGameObject.GetComponent<Combining_circle_ring>();
		move_slots_to_their_ring_radius(ring.transform);
		EditorSceneManager.MarkSceneDirty(Selection.activeGameObject.scene);
	}
	
	
	private void add_unit_slots (Combining_circle_ring ring, int slots_amount) {
		var example_slot = ring.unit_slots.First();
		add_children_along_a_circle(example_slot.transform, slots_amount);
		move_slots_to_their_ring_radius(ring.transform);

	}
	
	private void create_children_along_a_circle_for_selected_child() {
		var slots_number = Int32.Parse(number_edit.value);
		var copied_child = Selection.activeGameObject.GetComponent<Transform>();
		add_children_along_a_circle(copied_child, slots_number);
		move_slots_to_their_ring_radius(copied_child.parent);
		EditorSceneManager.MarkSceneDirty(Selection.activeGameObject.scene);
	}
	private void add_children_along_a_circle (Transform first_child, int children_amount) {
		var ring_radius = first_child.transform.position.distance_to(first_child.position);
		var step_angle = 360 / children_amount;
		for (var i_slot = 1; i_slot < children_amount; i_slot++) {
			var new_slot = 
				Object.Instantiate(
					first_child, 
					first_child.parent, 
					false
				);

			new_slot.name = first_child.name + $"{i_slot + 1}";
			
		}

	}

	private void move_slots_to_their_ring_radius(Transform ring) {
		var slots = ring.GetComponentsInChildren<Combining_circle_slot>();
		var example_slot = slots.First();
		var example_slot_rotation = example_slot.transform.localRotation * new Degree(180).to_quaternion();
		var ring_radius = example_slot.transform.position.distance_to(ring.transform.position);
		var step_angle = 360 / slots.Length;
		
		for (int i_slot=1; i_slot< slots.Length; i_slot++) {
			var slot_transform = slots[i_slot].transform;
			
			slot_transform.transform.localPosition =
				new Degree(i_slot*step_angle).to_quaternion()* Vector2.right * ring_radius;
			slot_transform.transform.rotation = slot_transform.transform.position.degrees_to(ring.transform.position).to_quaternion() * example_slot_rotation;
		}
	}


}
 