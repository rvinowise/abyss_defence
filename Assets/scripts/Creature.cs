using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PolygonCollider2D))]
public class Creature : MonoBehaviour
{
    Divisible_body divisible_body;

    void Start()
    {
        divisible_body = gameObject.GetComponent<Divisible_body>();
    }

    void Update()
    {
        
    }

    void OnMouseDown() {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Ray2D ray = new Ray2D(
                mousePos,
                new Vector2(0.5f,1f)
            );
        Divisible_body divisible_body = GetComponent<Divisible_body>(); 
        divisible_body.split_by_ray(ray);
        /*divisible_body.split_by_ray(
            new Ray2D(
                Camera.main.ScreenToWorldPoint(
                    new Vector2(Input.mousePosition.x, 
                                Input.mousePosition.y)
                ),
                new Vector2(0.5f,1f)
            )
        );*/
    }

    
}
