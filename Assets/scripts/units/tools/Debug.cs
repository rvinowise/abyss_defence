using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using geometry2d;


namespace debug {

public abstract class Debugger {
    
    public System.Object obj;

    public Debugger(System.Object in_object) {
        obj = in_object;
    }

    protected abstract ref int count { get; }

    public void increase_counter() {
        Interlocked.Increment(ref count);
        UnityEngine.Debug.Log(obj.GetType()+"added ("+count +")");
    }
    public void decrease_counter() {
        Interlocked.Decrement(ref count);
        UnityEngine.Debug.Log(obj.GetType()+"destroyed ("+count +")");
    }
}
}