using UnityEngine;


namespace rvinowise.unity {

public class Trajectory_flyer: MonoBehaviour {

    public float weight = 0.1f;
    [HideInInspector]
    public float size = 1f;

    public float height;
    [HideInInspector]
    public float vertical_velocity = 0f;


    public UnityEngine.Events.UnityEvent on_fell_on_the_ground;
    
    public bool is_on_the_ground() {
        return height <= 0f;
    }

    private void Update() {

        height += vertical_velocity * Time.deltaTime;
        vertical_velocity -= weight * Time.deltaTime;
        
        
        
        if (is_on_the_ground()) {
            this.enabled = false;
            if (on_fell_on_the_ground != null) {
                transform.localScale = new Vector2(size, size);
                on_fell_on_the_ground.Invoke();
            }
        }
        
        float local_scale = size * (height + 1);
        transform.localScale = new Vector2(local_scale, local_scale);

        
    }
    

}
}