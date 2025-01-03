using System;
using UnityEngine;

public class Camera_follower : MonoBehaviour
{

    public Transform target;
    public Vector2 central_rect = new Vector2(3f,2f);

    private Vector3 old_target_position;


    private void Awake() {
        old_target_position = target.position;
    }

    Vector3 calculate_target_moving_vector() {
        return target.position - old_target_position;
    }
    
    public float foreshadowing_distance = 40;
    Vector3 calculate_camera_perfect_spot() {
        return target.position + calculate_target_moving_vector() * foreshadowing_distance;
    }

    private Vector3 camera_perfect_spot;

    private void FixedUpdate() {
        if (old_target_position != target.position) {
            camera_perfect_spot = calculate_camera_perfect_spot();
            old_target_position = target.position;
        }
    }

    void Update()
    {
        if (is_outside_central_rect(camera_perfect_spot)) {
            Vector3 diff = (Vector2)(camera_perfect_spot - transform.position);
            transform.position += (diff/1.2f)*Time.deltaTime;
        }
    }

    private bool is_outside_central_rect(Vector3 target) {
        Vector3 diff = target - transform.position;
        return 
        (
            (central_rect.x < Mathf.Abs(diff.x)) ||
            (central_rect.y < Mathf.Abs(diff.y))
        );
    }

#if UNITY_EDITOR
    private void OnDrawGizmos() {
        // Gizmos.DrawLine(
        //     new Vector2(transform.position.x-central_rect.x,transform.position.y+central_rect.y/2), 
        //     new Vector2(transform.position.x+central_rect.x,transform.position.y+central_rect.y/2)
        //     );
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, central_rect*2);
        Gizmos.DrawLine(target.position, camera_perfect_spot);
    }
#endif
}
