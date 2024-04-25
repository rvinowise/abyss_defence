using System;
using UnityEngine;

public class Camera_follower : MonoBehaviour
{

    public Transform target;
    public Vector2 central_rect = new Vector2(3f,2f);
    void Start()
    {
        
    }

    void Update()
    {
        if (is_outside_central_rect(target)) {
            Vector3 diff = target.position - transform.position;
            diff.z = 0;
            transform.position += (diff/2)*Time.deltaTime;
        }
    }

    private bool is_outside_central_rect(Transform target) {
        Vector3 diff = target.position - transform.position;
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
    }
#endif
}
