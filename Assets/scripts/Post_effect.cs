using UnityEngine;

[ExecuteInEditMode]
public class Post_effect : MonoBehaviour
{
    public Material material;
    void OnRenderImage(RenderTexture src, RenderTexture dst) {


        Graphics.Blit(src, dst, material);
    }
}
