using System.Threading;

namespace rvinowise.units.debug {

public abstract class Debugger {
    public static bool is_off = false; //MANU
    
    public System.Object obj;

    public Debugger(System.Object in_object) {
        obj = in_object;
    }

    protected abstract ref int count { get; }

    public void increase_counter() {
        if (is_off) {
            return;
        }
        Interlocked.Increment(ref count);
        UnityEngine.Debug.Log(obj.GetType()+" added ("+count +")");
    }
    public void decrease_counter() {
        if (is_off) {
            return;
        }
        Interlocked.Decrement(ref count);
        UnityEngine.Debug.Log(obj.GetType()+" destroyed ("+count +")");
    }
}
}