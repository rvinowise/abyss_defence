using rvinowise.unity.units.humanoid;
using UnityEngine;


namespace rvinowise.unity.units.parts.tools {
public class Pickable : MonoBehaviour
{
    public Tool content;

    void Start()
    {
        content = GetComponentInChildren<Tool>();
    }

    void Update()
    {
        
    }

    void OnCollisionEnter2D(Collision2D collision) {
        if (collision.get_user_of_tools() is Humanoid user) {
            if (content is Ammunition ammo) {
                user.pick_up(content);
            }
            Destroy(gameObject);
        }
    }
}


}