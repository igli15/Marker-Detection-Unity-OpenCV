/* 
*   NatCam Core
*   Copyright (c) 2016 Yusuf Olokoba
*/

#if UNITY_5_3_4 || UNITY_5_3_5 || UNITY_5_3_6 || UNITY_5_4_OR_NEWER
	#define VERTEX_HELPER
#endif

using UnityEngine;

namespace NatCamU.Core.UI {

    using UnityEngine.UI;

    [RequireComponent(typeof(RawImage))]
    public sealed class NatCamPreview : MonoBehaviour, IMeshModifier { // NCDOC

        #region --Op vars--
        private ScaleMode scaleMode = 0;
        private Vector2 dimensions = Vector2.zero;
        private RawImage image {get {_image = _image ?? GetComponent<RawImage>(); return _image;}} // Used by editor
        private RawImage _image;
        private Material currMat, viewMat;
        #endregion


        #region --Properties--

        #if false
        public float Zoom {
            set {
                image.materialForRendering.SetFloat("_Zoom", value);
            }
        }
        #endif
        #endregion


        #region --Client API--

        public void Apply (Texture texture, Orientation orientation = Orientation.Rotation_0, ScaleMode scaleMode = ScaleMode.FillView) {
            // Set
            image.texture = texture;
            // Scale
            dimensions = texture ? OrientedDimensions(texture, orientation) : image.rectTransform.rect.size;;
            this.scaleMode = scaleMode;
            image.SetAllDirty();
            // Orient
            image.materialForRendering.SetFloat("_Rotation", ((int)orientation & 7) * 0.5f);
            image.materialForRendering.SetFloat("_Mirror", (int)orientation >> 3);
        }
        #endregion


        #region --Unity Messaging--

        private void Awake () {
            currMat = image.material;
            viewMat = new Material(Shader.Find("Hidden/NatCam/Transform2D"));
            viewMat.SetFloat("_Zoom", 1f);
            image.material = viewMat;
        }

        private void OnDestroy () {
            if (image) image.material = currMat;
            Destroy(viewMat);
        }
        #endregion


        #region --Unity UI Callbacks--
        
        void IMeshModifier.ModifyMesh (VertexHelper helper) {
            #if VERTEX_HELPER
            Vector3[] verts = CalculateVertices();
            UIVertex[] quad = new UIVertex[4];
            UIVertex vert = UIVertex.simpleVert;
            //Get the color
            Color color = image == null ? Color.white : image.color;
            //Vert0
            vert.position = verts[0];
            vert.uv0 = new Vector2(0f, 0f);
            vert.color = color;
            quad[0] = vert;
            //Vert1
            vert.position = verts[1];
            vert.uv0 = new Vector2(0, 1);
            vert.color = color;
            quad[1] = vert;
            //Vert2
            vert.position = verts[2];
            vert.uv0 = new Vector2(1, 1);
            vert.color = color;
            quad[2] = vert;
            //Vert3
            vert.position = verts[3];
            vert.uv0 = new Vector2(1, 0f);
            vert.color = color;
            quad[3] = vert;
            //Helper
            helper.Clear();
            helper.AddUIVertexQuad(quad);
            #endif
        }

        void IMeshModifier.ModifyMesh (Mesh mesh) {
            #if !VERTEX_HELPER
            var list = new System.Collections.Generic.List<Vector3>(CalculateVertices());
            mesh.SetVertices(list);
            #endif
        }
        #endregion
        

        #region --Panel Scaling--

        private Vector3[] CalculateVertices () {
            Vector2 corner1 = Vector2.zero;
            Vector2 corner2 = Vector2.one;
            float width, height;
            CalculateExtents(out width, out height);
            //Scale
            corner1.x *= width;
            corner1.y *= height;
            corner2.x *= width;
            corner2.y *= height;
            //Create a pivot vector, and pivot compensation vector
            Vector3 piv = new Vector3(image.rectTransform.pivot.x, image.rectTransform.pivot.y, 0f), comp = new Vector3(piv.x * width, piv.y * height, 0f);
            Vector3[] verts = new [] {
                new Vector3(corner1.x, corner1.y, 0f) - comp,
                new Vector3(corner1.x, corner2.y, 0f) - comp,
                new Vector3(corner2.x, corner2.y, 0f) - comp,
                new Vector3(corner2.x, corner1.y, 0f) - comp
            };
            return verts;
        }

        private void CalculateExtents (out float width, out float height) {
            width = height = 0;
            if (image == null) return;
			dimensions = dimensions == Vector2.zero ? image.rectTransform.rect.size : dimensions;
            float
			aspect = dimensions.x / dimensions.y,
			viewAspect = image.rectTransform.rect.size.x / image.rectTransform.rect.size.y;
            switch (scaleMode) {
                case ScaleMode.AdjustWidth :
                    height = image.rectTransform.rect.height;
                    width = height * aspect;
                break;
                case ScaleMode.AdjustHeight :
                    width = image.rectTransform.rect.width;
                    height = width / aspect;
                break;
                case ScaleMode.Letterbox :
                    if (aspect < viewAspect)
                    goto case ScaleMode.AdjustWidth;
                    else goto case ScaleMode.AdjustHeight;
				case ScaleMode.FillView :
					if (aspect > viewAspect)
					goto case ScaleMode.AdjustWidth;
					else goto case ScaleMode.AdjustHeight;
                case ScaleMode.FillScreen :
                    float scale = Mathf.Max(image.canvas.pixelRect.width / dimensions.x, image.canvas.pixelRect.height / dimensions.y) / image.canvas.scaleFactor;
					width = scale * dimensions.x;
                    height = scale * dimensions.y;
                break;
                case ScaleMode.None :
                    width = image.rectTransform.rect.width;
                    height = image.rectTransform.rect.height;
                break;
            }
        }

        private static Vector2 OrientedDimensions (Texture tex, Orientation orientation) {
            int rotation = (int)orientation & 7;
            return new Vector2(rotation % 2 == 0 ? tex.width : tex.height, rotation % 2 == 0 ? tex.height : tex.width);
        }
        #endregion
    }
}