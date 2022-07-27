using System;

namespace rvinowise.unity.geometry2d {

[Serializable]
public struct Dimension {
    public int width;
    public int height;

    public Dimension(int w, int h) {
        width = w;
        height = h;
    }
}

[Serializable]
public struct Dimensionf {
    public float width;
    public float height;

    public Dimensionf(float w, float h) {
        width = w;
        height = h;
    }
}

}