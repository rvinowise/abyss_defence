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


    public void explode() {
        Instantiate(explosion_prefab, transform.position, transform.rotation);
    }

    public void on_start_dying() {
        explode();
        Destroy(gameObject);
    }
}


}

