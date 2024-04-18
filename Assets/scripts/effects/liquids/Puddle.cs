using rvinowise.unity.extensions.pooling;

using UnityEngine;
using rvinowise.unity.extensions;
using rvinowise.unity.extensions.attributes;


namespace rvinowise.unity {

public class Puddle: MonoBehaviour {

    public float size;


    protected virtual void Awake() {
        transform.scale(size);
    }

    

}
}