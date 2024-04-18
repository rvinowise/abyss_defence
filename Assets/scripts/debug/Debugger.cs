using System.Threading;

namespace rvinowise.unity.debug {

public abstract class Debugger {
    public static bool is_off = false; //MANU
    
    protected abstract ref int count { get; }

    public void increase_counter() {
        if (is_off) {
            return;
        }
        Interlocked.Increment(ref count);
    }
    public void decrease_counter() {
        if (is_off) {
            return;
        }
        Interlocked.Decrement(ref count);
    }
}
}