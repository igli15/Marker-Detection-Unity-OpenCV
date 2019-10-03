using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;

public class PlaceCubesArcc : MonoBehaviour {
    [Serializable]
    public struct cubeObject {
        public GameObject cube;
        [Range( 0, 4 )]
        public float spacingScale;
        [ReadOnly] public float mySpacing;
        [ReadOnly] public float totalSpacing;
    }
    [Range( 1, 10 )]
    public int numberOfCubes;
    public List<cubeObject> cubeObjects = new List<cubeObject>();

    public float radius;
    public float degreesBetweenCubes = 1;

    //[ReadOnly]
    //public float currrentAverage = 0;
    [ReadOnly]
    public float currentHalfLength = 0;
    [ReadOnly]
    public float spacingDegrees = 0;
    [ReadOnly]
    public float spacingRad;

    private void Update() {

    }

    private float GetAverageSpacing() {
        float average = 0;
        foreach ( cubeObject s in cubeObjects ) {
            average += s.spacingScale;
        }
        if ( cubeObjects.Count == 0 ) return 0;

        average /= cubeObjects.Count;
        return average;
    }





    [Button]
    public void WrapCubes() {
        spacingRad = degreesBetweenCubes * Mathf.Deg2Rad;
        Debug.Log( "wrapping.." );


        float halfLength = GetTotalSpacesForCubes( numberOfCubes -1 )/2;
        currentHalfLength = halfLength * Mathf.Rad2Deg;
        //angle -= (Mathf.PI * currentSpacing) / 2;

        for ( int i = 0; i < cubeObjects.Count; i++ ) {
            cubeObject c = cubeObjects[ i ];
            if ( i >= numberOfCubes ) {
                c.cube.SetActive( false );
                continue;
            }
            c.cube.SetActive( true );

            if ( i == 0 ) {
                c.spacingScale = 0;
            }
            c.mySpacing = degreesBetweenCubes * c.spacingScale;
            c.totalSpacing = GetTotalSpacesForCubes( i );

            float angle = c.totalSpacing ;
            angle += Mathf.PI / 2;
            angle -= halfLength ;

            float x = radius * Mathf.Cos( angle );
            float y = radius * Mathf.Sin( angle );

            Vector3 pos = new Vector3( x, y - radius, transform.position.x );
            c.cube.transform.position = pos;
            c.cube.transform.rotation = Quaternion.identity;
            c.cube.transform.Rotate( new Vector3( 0, 0, angle * Mathf.Rad2Deg - 90 ) );
            cubeObjects[ i ] = c;
        }
    }

    public float GetTotalSpacesForCubes( int cubeIndex ) {
        float totalSpacing = 0;
        for ( int i = 1; i <= cubeIndex; i++ ) {
            cubeObject c = cubeObjects[ i ];
            totalSpacing += spacingRad * c.spacingScale;
        }
        return totalSpacing;
    }
}
