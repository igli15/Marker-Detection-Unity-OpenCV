using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using Sirenix.OdinInspector;

[ExecuteInEditMode]
public class CurveTextMeshPro : MonoBehaviour
{
    [Serializable]
    public struct CharacterSettings
    {
        [Range(0, 4)]
        public float spacingScale;
        [ReadOnly]
        public char character;
        [ReadOnly]
        public float mySpacing;
        [ReadOnly]
        public float totalSpacing;
    }

    public List<CharacterSettings> characters = new List<CharacterSettings>();

    [Range(0,20)]
    public float radius = 1;

    public float degreesBetweenCharacters = 1;

    [ReadOnly]
    public float currentHalfLength = 0;
    [ReadOnly]
    public float spacingDegrees = 0;
    [ReadOnly]
    public float spacingRad;

    private TMP_Text textComponent;

    void Awake()
    {
        textComponent = GetComponent<TMP_Text>();

        Debug.Assert(textComponent != null); //Make sure we have a TMP_TEXT component!
    }

    [Button]
    public void GenerateCharacterSettings()
    {
        int charCount = textComponent.textInfo.characterCount;
        characters.Clear();

        for (int i = 0; i < charCount; i++)
        {
            char c = textComponent.textInfo.characterInfo[i].character;
            characters.Add(new CharacterSettings() { spacingScale = 1, character = c});
        }
    }

    [Button]
    public void CurveText()
    {
        Debug.Log("Curving Text....");
      
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
            Debug.LogWarning("The text is empty!");
            return;
        }

        //Get the min/max horizontal points (bounds)
        float minBoundX = textComponent.bounds.min.x;
        float maxBoundX = textComponent.bounds.max.x;

        float minBoundY = textComponent.bounds.min.y;
        float maxBoundY = textComponent.bounds.max.y;

        spacingRad = degreesBetweenCharacters * Mathf.Deg2Rad;

        float halfLength = GetTotalSpaces(charCount - 1) / 2;
        currentHalfLength = halfLength * Mathf.Rad2Deg;

        //Loop all the characters
        for (int i = 0; i < textInfo.characterCount; i++)
        {
            //Skip if the current character is not displaying
            if(!textInfo.characterInfo[i].isVisible)
            {
                Debug.Log("Skiping character with index: " + i + ". It is not visible");
                continue;
            }

            CharacterSettings characterSettings = characters[i];

            if(i == 0)
            {
                characterSettings.spacingScale = 0;
            }

            characterSettings.mySpacing = degreesBetweenCharacters * characterSettings.spacingScale;
            characterSettings.totalSpacing = GetTotalSpaces(i);

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

            vertices[vertexIndex + 0] += -localCenter;
            vertices[vertexIndex + 1] += -localCenter;
            vertices[vertexIndex + 2] += -localCenter;
            vertices[vertexIndex + 3] += -localCenter;

            Debug.DrawLine(transform.TransformPoint(localCenter), transform.TransformPoint(localCenter - new Vector3(0,2,0)),Color.green,2);


            float angle = characterSettings.totalSpacing;
            angle += Mathf.PI / 2;
            angle -= halfLength;

            float x = radius * -Mathf.Cos(angle);
            float y = radius * Mathf.Sin(angle);

            characters[i] = characterSettings;

            //Apply our matrixes (apply position and rotation)
            matrix = Matrix4x4.TRS(new Vector3(x, y, 0), Quaternion.Euler(0,0, 90 - angle * Mathf.Rad2Deg), Vector3.one);

            vertices[vertexIndex + 0] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 0]);
            vertices[vertexIndex + 1] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 1]);
            vertices[vertexIndex + 2] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 2]);
            vertices[vertexIndex + 3] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 3]);

            //move it down by radius that the top border is always the max y
            vertices[vertexIndex + 0] -= new Vector3(0, radius,0);
            vertices[vertexIndex + 1] -= new Vector3(0, radius, 0);
            vertices[vertexIndex + 2] -= new Vector3(0, radius, 0);
            vertices[vertexIndex + 3] -= new Vector3(0, radius, 0);
           
        }
        textComponent.UpdateVertexData();
    }


    private Vector2 GetTheCenterOfTheText()
    {
        TMP_TextInfo textInfo = textComponent.textInfo;
        int i = textComponent.textInfo.characterCount/2 -1;

        Vector3[] vertices;
        int vertexIndex = textComponent.textInfo.characterInfo[i].vertexIndex;
        int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;
        vertices = textInfo.meshInfo[materialIndex].vertices;
       
        Vector3 localCenter = new Vector2((vertices[vertexIndex].x + vertices[vertexIndex + 2].x) / 2, (vertices[vertexIndex].y + vertices[vertexIndex + 2].y) / 2);


        // Apply offset to adjust our pivot point.
        vertices[vertexIndex + 0] += -localCenter;
        vertices[vertexIndex + 1] += -localCenter;
        vertices[vertexIndex + 2] += -localCenter;
        vertices[vertexIndex + 3] += -localCenter;

        return new Vector2((vertices[vertexIndex].x +vertices[vertexIndex + 2].x) / 2, (vertices[vertexIndex].y + vertices[vertexIndex + 2].y) / 2); ;

    }

    public float GetTotalSpaces(int characterIndex)
    {
        float totalSpacing = 0;
        for (int i = 1; i <= characterIndex; i++)
        {
            CharacterSettings c = characters[i];
            totalSpacing += spacingRad * c.spacingScale;
        }
        return totalSpacing;
    }
}
