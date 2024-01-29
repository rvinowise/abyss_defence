


using UnityEngine;
using rvinowise.contracts;

namespace rvinowise.unity.maps {

public class Map: MonoBehaviour {

    public UnityEngine.Rect rect;

    public static Map instance;

    private void Awake() {
        Contract.Requires(instance == null, "Map is a singleton");
        instance = this;
    }

    public bool has(
        Transform transform
    ) {
        if (rect.Contains(transform.position)) {
            return true;
        }
        return false;
    }

    public float ground_z;

}

}