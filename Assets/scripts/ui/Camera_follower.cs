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
            (Mathf.Abs(central_rect.x) < diff.x) ||
            (Mathf.Abs(central_rect.y) < diff.y)
        );
    }
}
