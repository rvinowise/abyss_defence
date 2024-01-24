using UnityEngine;
using UnityEditor;

namespace rvinowise.unity.debug {

public static class Debug {

    public static void Assert(bool in_condition) {
        if (!in_condition) {
            UnityEngine.Debug.Assert(false);
        }
    }

    public static void DrawLine(
        Vector3 p1,
        Vector3 p2,
        Color color,
        float width = 1
    ) {
        Handles.DrawBezier(p1,p2,p1,p2, color, null,width);
    }
    public static void DrawLine_simple(
        Vector3 p1,
        Vector3 p2,
        Color color,
        float width = 1,
        float time = 0
    )
    {

        int count = 1 + Mathf.CeilToInt(width); // how many lines are needed.
        if (count == 1)
        {
            //Gizmos.DrawLine(p1, p2);
            UnityEngine.Debug.DrawLine(p1, p2, color, time);
        }
        else
        {
            Camera c = Camera.main;
            if (c == null)
            {
                UnityEngine.Debug.LogError("Camera is null");
                return;
            }
            var scp1 = c.WorldToScreenPoint(p1);
            var scp2 = c.WorldToScreenPoint(p2);

            Vector3 v1 = (scp2 - scp1).normalized; // line direction
            Vector3 n = Vector3.Cross(v1, Vector3.forward); // normal vector

            for (int i = 0; i < count; i++)
            {
                Vector3 o = 0.99f * n * width * ((float)i / (count - 1) - 0.5f);
                Vector3 origin = c.ScreenToWorldPoint(scp1 + o);
                Vector3 destiny = c.ScreenToWorldPoint(scp2 + o);
                UnityEngine.Debug.DrawLine(origin, destiny, color, time);
            }
        }
    }

}
}