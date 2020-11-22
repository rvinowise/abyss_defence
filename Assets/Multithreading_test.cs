using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Multithreading_test : MonoBehaviour
{

    Vector2 position;
    void Start()
    {
        //Thread thread = new Thread(Run);
        //thread.Start();
    }

    private void Run() {
        while(true) {
            MoveObject();
            Thread.Sleep(50);
        }
    }

    private void MoveObject() {
        position += Vector2.right * 0.01f;
    }

    void Update()
    {
        transform.position = position;
    }
}
