using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ChromaticEffect : MonoBehaviour
{
   
    public Vector2 redOffset;
    public Vector2 greenOffset;
    public Vector2 blueOffset;

    public float effectDistance;

    private Material material;
    private Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        material = new Material(Shader.Find("Hidden/ChromaticShader"));
        cam = GetComponent<Camera>();
        cam.depthTextureMode = cam.depthTextureMode | DepthTextureMode.Depth;
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        material.SetVector("u_redOffset", redOffset);
        material.SetVector("u_greenOffset", greenOffset);
        material.SetVector("u_blueOffset", blueOffset);
        material.SetFloat("u_effectDistance", effectDistance);
        Graphics.Blit(source, destination, material);
    }

    
}
