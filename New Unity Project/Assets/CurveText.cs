using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Sirenix.OdinInspector;

[ExecuteInEditMode]
public class CurveText : MonoBehaviour
{
    public bool isCanvasElement =true;

    [SerializeField]
    [ShowIf("isCanvasElement")]
    private RectTransform canvasRectTransform;

    //radius of the curve
    public float radius = 8;

    //divide each vertice world location based on this value
    public Vector2 verticesDividor = Vector2.one * 50;

    //max local space of the text
    [ReadOnly]
    public float maxPos;

    //min local space of the text
    [ReadOnly]
    public float minPos;

    private TMP_Text textComponent;

    private float scaledRadius;
    private Vector2 scaledverticesDividor;


    void Awake()
    {
        textComponent = GetComponent<TMP_Text>();
    }

    [Button(40)]
    public void Curve()
    {

        if (isCanvasElement)
        {
            scaledRadius = radius * 100;
            scaledverticesDividor.x = verticesDividor.x * canvasRectTransform.rect.width;
            scaledverticesDividor.y = verticesDividor.y * canvasRectTransform.rect.width;
        }
        else
        {
            scaledRadius = radius;
            scaledverticesDividor = verticesDividor;
        }

        Debug.Log("Curving Text....");

        Debug.Assert(textComponent != null);
        //All the text mesh vertices will be stored here
        Vector3[] vertices;

        //will be used in the end to apply our transformations
        Matrix4x4 matrix;

        //updates TMP_TEXT so we can get the latest mesh data
        textComponent.ForceMeshUpdate();

        //Get Text information and character count
        TMP_TextInfo textInfo = textComponent.textInfo;
        int charCount = textInfo.characterCount;

        //No need to continue if the text has not characters
        if (charCount == 0)
        {
            Debug.LogWarning("The text you are trying to curve is empty!");
            return;
        }

        //Loop all the characters
        for (int i = 0; i < textInfo.characterCount; i++)
        {
            //Skip if the current character is not displaying
            if (!textInfo.characterInfo[i].isVisible)
            {
                Debug.Log("Skiping character with index: " + i + ". It is not visible");
                continue;
            }

            //Get the index of the vertex for this character
            int vertexIndex = textInfo.characterInfo[i].vertexIndex;
            //Get the material index
            int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;

            //store the vertices of the text.
            //Each character has 4 vertices TL,TR,BL,BR. here we get the total vertices
            vertices = textInfo.meshInfo[materialIndex].vertices;

            //if we do vertices[vertexIndex] that gives the first vertice of the character.vertice index
            //if we fo vertices[vertexIndex + 1] that gives the second and so on until we have all 4 (0,1,2,3)

            //we start at the first vertice per character then we add to since we need only the horizontal so we go from left to right
            Vector3 localCenter = new Vector2((vertices[vertexIndex].x + vertices[vertexIndex + 2].x) / 2, (vertices[vertexIndex].y + vertices[vertexIndex + 2].y) / 2);

            if (i == 0) minPos = localCenter.x;
            if (i == charCount - 1) maxPos = localCenter.x;

            Vector3 finalPos0 = vertices[vertexIndex + 0];
            Vector3 finalPos1 = vertices[vertexIndex + 1];
            Vector3 finalPos2 = vertices[vertexIndex + 2];
            Vector3 finalPos3 = vertices[vertexIndex + 3];

            //Move each vertex to the circle based on the radius and it's loal dividor
            //lets say if x of one character is 9 and you divide by 9 that with be position on Sin(1).
            //move each vertex this way so the hole character is postion correctly.
            //Y is inverted for some reason acts weird.
            finalPos0.x += (scaledRadius * -Mathf.Sin(finalPos0.x / scaledverticesDividor.x));
            finalPos0.y += (scaledRadius * Mathf.Cos(finalPos0.y / scaledverticesDividor.y));


            finalPos1.x += (scaledRadius * -Mathf.Sin(finalPos1.x / scaledverticesDividor.x));
            finalPos1.y += (scaledRadius * Mathf.Cos(finalPos1.y / scaledverticesDividor.y));


            finalPos2.x += (scaledRadius * -Mathf.Sin(finalPos2.x / scaledverticesDividor.x));
            finalPos2.y += (scaledRadius * Mathf.Cos(finalPos2.y / scaledverticesDividor.y));


            finalPos3.x += (scaledRadius * -Mathf.Sin(finalPos3.x / scaledverticesDividor.x));
            finalPos3.y += (scaledRadius * Mathf.Cos(finalPos3.y / scaledverticesDividor.y));

            //Change the pivot (Only X axis) of the character to be in the center (need this for the rotation).
            //Ignore y Axis pivots since we always want them to be leveld on the bottom.
            vertices[vertexIndex + 0].x = finalPos0.x  - localCenter.x;
            vertices[vertexIndex + 0].y = finalPos0.y ;
           

            vertices[vertexIndex + 1].x = finalPos1.x - localCenter.x;
            vertices[vertexIndex + 1].y = finalPos1.y ;
           

            vertices[vertexIndex + 2].x = finalPos2.x - localCenter.x;
            vertices[vertexIndex + 2].y = finalPos2.y ;
           

            vertices[vertexIndex + 3].x = finalPos3.x - localCenter.x;
            vertices[vertexIndex + 3].y = finalPos3.y ;

            //Calculate our anlge of rotation
            float angle = scaledRadius *  localCenter.x/ -scaledverticesDividor.x ;

            //Apply our matrixes (applies rotation)
            matrix = Matrix4x4.TRS(new Vector3(0, 0, 0), Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg), Vector3.one);

            vertices[vertexIndex + 0] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 0]);
            vertices[vertexIndex + 1] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 1]);
            vertices[vertexIndex + 2] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 2]);
            vertices[vertexIndex + 3] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 3]);

            //move it down by radius that the top border is always the max y
            vertices[vertexIndex + 0] -= new Vector3(0, scaledRadius, 0);
            vertices[vertexIndex + 1] -= new Vector3(0, scaledRadius, 0);
            vertices[vertexIndex + 2] -= new Vector3(0, scaledRadius, 0);
            vertices[vertexIndex + 3] -= new Vector3(0, scaledRadius, 0);
          
        }
        textComponent.UpdateVertexData();
    }

    private void OnValidate()
    {
        Curve();
    }

}
