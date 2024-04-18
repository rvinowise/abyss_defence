using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using rvinowise.unity.extensions;
using rvinowise.unity.geometry2d;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;


namespace rvinowise.unity {
public class Combining_circle_ring : MonoBehaviour {

    public List<Combining_circle_slot> unit_slots;
    public ISet<int> filled_slots_indices = new HashSet<int>();

    public Turning_element_actor turning_element;
    public float radius=3;

    void Awake() {
        for(var i_slot=0; i_slot<unit_slots.Count;i_slot++) {
            if (unit_slots[i_slot].is_filled()) {
                filled_slots_indices.Add(i_slot);
            }
        }
    }


    public void free_slot_with_index(int index) {
        filled_slots_indices.Remove(index);
        unit_slots[index].filled = false;
    }
    public Combining_circle_slot retrieve_random_filled_slot() {
        var slot_index = get_random_filled_slot_index();
        free_slot_with_index(slot_index);
        return unit_slots[slot_index];
    }
    
    public int get_random_filled_slot_index() {
        return filled_slots_indices.ElementAt(Random.Range(0, filled_slots_indices.Count));
    }

    public Degree get_ring_direction_for_slot_direction(Combining_circle_slot slot, Degree needed_direction_to_slot) {
        var angle_to_slot = new Degree(slot.transform.localPosition.to_dergees());
        return angle_to_slot.angle_to(needed_direction_to_slot);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos() {
        for(var slot_index=0;slot_index<unit_slots.Count;slot_index++) {
            var slot = unit_slots[slot_index];
            
            GUIStyle myStyle = new GUIStyle {
                fontSize = 20,
                fontStyle = FontStyle.Bold
            };
            
            if (slot.is_filled()) {
                myStyle.normal.textColor = Color.green;
            }
            else {
                myStyle.normal.textColor = Color.red;
            }
        
            Handles.Label(slot.transform.position, slot_index.ToString(), myStyle);
        }
    }
#endif


}

    
}