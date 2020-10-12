using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise;


namespace rvinowise.rvi {

public  static class Debug {

    public static void Assert(bool in_condition) {
        if (!in_condition) {
            UnityEngine.Debug.Assert(false);
        }
    }

}
}