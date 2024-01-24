using UnityEngine;


namespace rvinowise.unity.management {
public class Persistent_everywhere : MonoBehaviour
{
   void Awake() {
       DontDestroyOnLoad(gameObject);
   }
}

}