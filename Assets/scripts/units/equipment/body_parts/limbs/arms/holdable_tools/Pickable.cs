using rvinowise.unity.extensions;
using UnityEngine;


namespace rvinowise.unity {
public class Pickable : MonoBehaviour
{
    public Tool content;

    void Start()
    {
        content = GetComponentInChildren<Tool>();
    }


    void OnCollisionEnter2D(Collision2D collision) {
        if (collision.get_user_of_tools() is Humanoid user) {
            user.pick_up(content);
            this.destroy();
        }
    }
}


}