using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace rvinowise.unity.management {
public class Persistent_everywhere : MonoBehaviour
{
   void Awake() {
       DontDestroyOnLoad(gameObject);
   }
}

}