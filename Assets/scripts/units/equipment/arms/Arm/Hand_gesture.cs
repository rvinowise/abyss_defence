using UnityEngine;


namespace rvinowise.unity.units.parts.limbs.arms {

public class Hand_gesture : Headspring.Enumeration<Hand_gesture, int> {
    public static readonly Hand_gesture Relaxed = new Hand_gesture(
        0,
        "Relaxed",
        new Vector2(0.11f, 0f)
    );
    public static readonly Hand_gesture Grip_of_vertical = new Hand_gesture(
        1, 
        "Grip_of_vertical",
        new Vector2(0.11f, 0f)
    );
    
    public static readonly Hand_gesture Support_of_horizontal = new Hand_gesture(
        2, "Support_of_horizontal", new Vector2(0.13f, 0f));
    
    public static readonly Hand_gesture Open_sideview = new Hand_gesture(3, "Open_sideview");

    public static readonly Hand_gesture Hold_gun_sideview = new Hand_gesture(
        4, 
        "Hold_gun_sideview",
        new Vector2(0.11f, 0f)
    );
    
    public static readonly Hand_gesture Hold_horizontal_flat_object = new Hand_gesture(
        5, 
        "Hold_horizontal_flat_object",
        new Vector2(0.11f, 0f)
    );
    
    private Hand_gesture(int value, string displayName) : base(value, displayName) {
        this.valuable_point = new Vector2(0.11f, 0f);
    }

    private Hand_gesture(int value, string displayName, Vector2 valuable_point) :
        base(value, displayName) {
        this.valuable_point = valuable_point;
    }


    public Vector2 valuable_point { get; set; }
}
}