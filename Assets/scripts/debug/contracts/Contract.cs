//#define DEBUG
//#define UNITY_ASSERTIONS

using System;
using System.Diagnostics;


namespace rvinowise.rvi.contracts {
public class Contract {
    
    public static void Requires( bool condition, string message="")
    {
#if RVI_CONTRACTS
            #region debug
            if (!condition)
            {
                bool test = true;
            }
            #endregion
            UnityEngine.Debug.Assert(condition, message);
        //UnityEngine.Assertions.Assert.IsTrue(condition);
#endif
    }
    
    /* not strict requirement, but most logical use of the code.
     if broken, something is not optimal */
    public static void Assume( bool condition, string message="")
    {
#if RVI_CONTRACTS
        #region debug
        if (!condition)
        {
            bool test = true;
        }
        #endregion
        UnityEngine.Debug.Assert(condition, message);
        //UnityEngine.Assertions.Assert.IsTrue(condition);
#endif
    }
    
    
    /* a substitute for native asserts, because they don't stop at breakpoints */
    public static void Assert( bool condition, string message="")
    {
#if RVI_CONTRACTS
        #region debug
        if (!condition)
        {
            bool test = true;
        }
        #endregion
        UnityEngine.Debug.Assert(condition, message);
        //UnityEngine.Assertions.Assert.IsTrue(condition);
#endif
    }
    
    /* the postcondition before exit a function */
    public static void Ensures( bool condition, string message="")
    {
#if RVI_CONTRACTS
        #region debug
        if (!condition)
        {
            bool test = true;
        }
        #endregion
        UnityEngine.Debug.Assert(condition, message);
        //UnityEngine.Assertions.Assert.IsTrue(condition);
#endif
    }
}
}