


using System.Collections.Generic;
using UnityEngine;
using rvinowise.contracts;
using rvinowise.unity.extensions;


namespace rvinowise.unity {

public class Damaged_floor: MonoBehaviour {


    public static Damaged_floor instance;

    public List<GameObject> damage_prefabs = new List<GameObject>();

    private void Awake() {
        Contract.Requires(instance == null, "Damaged_floor is a singleton");
        instance = this;
    }


    public void damage_point(Vector2 hit_point, Vector2 impact_vector) {
        foreach (var damage_prefab in damage_prefabs) {
            Instantiate(damage_prefab, hit_point, impact_vector.to_quaternion(), null);
        }
    }


}

}