using UnityEngine;

public class Test : MonoBehaviour
{
    void Start()
    {
        
    }

    void FixedUpdate()
    {
        for (int i=0;i<1000;i++) {
            GameObject new_object = new GameObject();
            new_object.transform.parent = this.transform;
        }
    }
}
