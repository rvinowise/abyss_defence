//#define DEBUG
//#define UNITY_ASSERTIONS

using System;
using System.Diagnostics;


namespace rvinowise.rvi.contracts {
public class Contract {
    
    
    /*public static void Requires( bool condition, string message="" )
    {
#if RVI_CONTRACTS
        Requires<Broken_contract_exception>(condition);
#endif
    }

    public static void Requires<TException>( bool condition, string message="" )
        where TException : Exception, new()
    {
#if RVI_CONTRACTS
        if ( !condition )
        {
            Debug.WriteLine( message );
            throw new TException();
        }
#endif
    }*/
    
    public static void Requires( bool condition, string message="")
    {
#if RVI_CONTRACTS
        UnityEngine.Debug.Assert(condition, message);
        //UnityEngine.Assertions.Assert.IsTrue(condition);
#endif
    }
    
    /* not strict requirement, but most logical use of the code.
     if broken, something is not optimal */
    public static void Assume( bool condition, string message="")
    {
#if RVI_CONTRACTS
        UnityEngine.Debug.Assert(condition, message);
        //UnityEngine.Assertions.Assert.IsTrue(condition);
#endif
    }
}
}