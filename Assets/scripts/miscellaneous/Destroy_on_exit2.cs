using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise.unity.extensions;


public class DestroyOnExit : StateMachineBehaviour {
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        Destroy(animator.gameObject);
    }
}