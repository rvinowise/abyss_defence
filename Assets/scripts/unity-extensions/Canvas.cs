using System;
using UnityEngine;


namespace rvinowise.unity.extensions {
public static partial class Unity_extension {

    public static void add_item(
        this Canvas in_canvas,
        MonoBehaviour in_item
    ) {
        GameObject item_canvas_object = new GameObject();
        item_canvas_object.name = 
            String.Format("canvas {0}",in_item.name);
        Canvas item_canvas = item_canvas_object.add_component<Canvas>();
        item_canvas.transform.SetParent(in_canvas.transform, false);
        in_item.transform.SetParent(item_canvas.transform, false);
    }
}
}