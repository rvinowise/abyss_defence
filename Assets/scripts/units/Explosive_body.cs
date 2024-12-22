using System;
using System.Globalization;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace rvinowise.unity {

public class Explosive_body: 
    MonoBehaviour 
    ,IDestructible
{


    public GameObject explosion_prefab;
    public bool is_explosion_instantiated = false;

    public void create_explosion() {
        if (is_explosion_instantiated) {
            explosion_prefab.transform.SetParent(null,true);
            explosion_prefab.SetActive(true);
        } else {
            Instantiate(explosion_prefab, transform.position, transform.rotation);
        }
    }

    public void on_start_dying() {
        create_explosion();
        Destroy(gameObject);
    }
}


}

