using System;
using System.Collections;
using System.Collections.Generic;
using Headspring;
using UnityEngine;
using rvinowise;


namespace rvinowise.units.parts.limbs.arms {

public class Hand_gesture : Headspring.Enumeration<Hand_gesture, int> {
    public static readonly Hand_gesture Relaxed = new Hand_gesture(0, "Relaxed");
    public static readonly Hand_gesture Grip_of_vertical = new Hand_gesture(1, "Grip_of_vertical");
    public static readonly Hand_gesture Support_of_horizontal = new Hand_gesture(2, "Support_of_horizontal");
    public static readonly Hand_gesture Open_sideview = new Hand_gesture(3, "Open_sideview");

    private Hand_gesture(int value, string displayName) : base(value, displayName) { }



}
}