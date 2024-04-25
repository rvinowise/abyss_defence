using rvinowise.unity.extensions.pooling;

using UnityEngine;
using rvinowise.unity.extensions;
using rvinowise.unity.extensions.attributes;


namespace rvinowise.unity {

public class Disappearing_puddle: Puddle {


    //public Pooled_object pooled_object;

    public Animator animator;
    

    [called_in_animation]
    public void on_disappearing_ends() {
        Destroy(this.gameObject);
    }

    [called_in_animation]
    public void on_spreaded() {
        //animator.SetTrigger("start_disappearing");
    }

}
}