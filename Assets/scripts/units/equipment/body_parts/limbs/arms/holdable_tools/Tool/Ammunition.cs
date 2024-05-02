using System;
using UnityEngine;


namespace rvinowise.unity {

[RequireComponent(typeof(Tool))]
public class Ammunition: MonoBehaviour {
    public int rounds_qty;
    public int max_rounds_qty;
    public Ammo_compatibility compatibility;

    public Tool tool;

    private void Awake() {
        tool = GetComponent<Tool>();
    }

    public void deactivate() {
        tool.deactivate();
    }

}
}