using UnityEngine;
using rvinowise.unity.extensions.pooling;
using System.Collections.Generic;
using System.Linq;
using rvinowise.unity.geometry2d;

namespace rvinowise.unity.units.parts.weapons.thorny_mine {

[RequireComponent(typeof(Pooled_object))]
public class Thorny_mine: MonoBehaviour {

    public int thorns_qty;
    public float activation_delay = 1;
    public Thorn thorn_prefab;
    
    private IList<Thorn> thorns = new List<Thorn>();
    private Pooled_object pooled_object;

    void Awake() {
        pooled_object = GetComponent<Pooled_object>();
        init_thorns();
    }

    void OnEnable() {
        Invoke("emit_thorns", activation_delay);
    }

    private void init_thorns() {
        foreach (int i in Enumerable.Range(0, thorns_qty)) {
            Thorn new_thorn = GameObject.Instantiate(thorn_prefab);
            new_thorn.transform.SetParent(this.transform, false);
            thorns.Add(new_thorn);
        }
        
    }

    void Update() {

    }

    private void emit_thorns() {
        foreach (Thorn thorn in thorns) {
            Quaternion direction = Directions.degrees_to_quaternion(Random.value*360);
            place_thorn_for_direction(thorn, direction);
            thorn.go_off();
        }
    }

    private void place_thorn_for_direction(
        Thorn in_thorn, 
        Quaternion in_rotation
    ) {
        in_thorn.transform.rotation = in_rotation;
        float body_radius = 0.5f;
        in_thorn.transform.localPosition = in_rotation * new Vector2(in_thorn.max_length * body_radius, 0);
    }

}

}