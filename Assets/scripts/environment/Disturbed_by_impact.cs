using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Disturbed_by_impact : MonoBehaviour
{
    public Collider2D collider;

    void Awake() {
        collider = GetComponent<Collider2D>();
    }
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void OnTriggerStay2D(Collider2D other) {
        Vector2 position = other.transform.position;
        //Vector2 velocity = other.relativeVelocity;

        //Debug.DrawLine(position, position + velocity, Color.magenta);
    }

    public void OnCollisionStay2D(Collision2D collision) {
        ContactPoint2D main_contact = collision.GetContact(0);
        Vector2 position = main_contact.point;
        Vector2 velocity = main_contact.relativeVelocity;

        Debug.DrawLine(position, position + velocity, Color.magenta);
    }
}
