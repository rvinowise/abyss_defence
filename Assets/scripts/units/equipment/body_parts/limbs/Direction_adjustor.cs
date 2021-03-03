
using rvinowise.contracts;
using rvinowise.unity.units.parts.limbs;
using UnityEngine;


/* placed in a gameObject of the Image, which is a child of the Segment */
namespace rvinowise.unity.helpers.graphics {

    [RequireComponent(typeof(SpriteRenderer))]
    public class Direction_adjustor: MonoBehaviour {

        //[HideInInspector]
        public SpriteRenderer sprite_renderer;
        void Awake() {
            sprite_renderer = GetComponent<SpriteRenderer>();
            Contract.Assume(
                transform.parent.GetComponent<Segment>() != null,
                "This component adjusts direction of a sprite for the parent segment "
            );
            Contract.Assume(
                transform.localRotation != Quaternion.identity,
                "This component should rotate its sprite to be useful"
            );
            Contract.Assume(
                sprite_renderer != null,
                "sprite renderer is needed on the direction adjustor"
            );
        }
    }

}