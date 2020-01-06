using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
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
        divisible_body.divide_in_half(
            Camera.main.ScreenToWorldPoint(
                new Vector2(Input.mousePosition.x, 
                            Input.mousePosition.y)),
            new Vector2(1f,0.5f)
        );
        Destroy(gameObject);
    }
}
