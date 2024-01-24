using UnityEngine;


namespace rvinowise.unity.units.parts.weapons.thorny_mine {

public class Thorn: MonoBehaviour {

    public float extension_speed = 0.1f;
    public float max_length_from = 0.5f;
    public float max_ength_to = 1.5f;
    public Thorn_interval bottom_interval;
    public Thorn_interval top_interval;

    public float max_length;

    private Extend_state extend_state = Extend_state.compacted;
    void Awake() {
        max_length = max_length_from + Random.value * (max_ength_to - max_length_from);
    }

    void Update() {
        switch(extend_state) {
            case (Extend_state.bottom):
                extend(bottom_interval.transform);
                break;
            case (Extend_state.top):
                extend(top_interval.transform);
                break;
        }
    }

    private void extend(Transform in_interval) {
        Vector3 old_scale = in_interval.localScale;
        in_interval.localScale = new Vector3(
            old_scale.x + extension_speed*Time.deltaTime,
            old_scale.y,
            old_scale.z
        );
        if (reached_max_length()) {
            extend_state = Extend_state.fully_extended;
        }

        bool reached_max_length() {
            return 
                (
                    bottom_interval.transform.lossyScale.x +
                    top_interval.transform.lossyScale.x
                ) >= max_length;

        }
    }

    public void go_off() {
        extend_state = Extend_state.bottom;
    }

    enum Extend_state {
        compacted,
        bottom,
        top,
        fully_extended
    }

}

}