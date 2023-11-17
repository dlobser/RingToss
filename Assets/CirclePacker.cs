
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class CirclePacker
{

    public static float largeSize = .1f;
    public static float smallSize = .01f;
    public static int num = 250;
    public static float width = .5f;
    public static float height = .5f;
    static float[,] circmat;
    public static List<Vector3> positionAndRadius;
    // public static int randomSeed;
    /// <summary>
    /// Returns a list with x,y,radius of packed circls
    /// </summary>
    /// <returns>The circles.</returns>
    public static List<Vector3> PackCircles()
    {
        positionAndRadius = new List<Vector3>();
        circmat = initializeCoords(num);
        FindAllRadii(circmat, 2);
        return positionAndRadius;
    }

    /// <summary>
    /// Returns a list with x,y,radius of packed circls
    /// </summary>
    /// <returns>The circles.</returns>
    /// <param name="small">smallest circle.</param>
    /// <param name="large">largest circle.</param>
    /// <param name="amt">Amount of circles.</param>
    /// <param name="w">World extents.</param>
    
    public static List<Vector3> PackCircles(float small, float large, int amt, float w, float h = 1, List<Vector3> initPositions = null)
    {
        smallSize = small;
        largeSize = large;
        num = amt;
        width = w;
        height = h;
        positionAndRadius = new List<Vector3>();
        
        Random.InitState(GlobalSettings.randomSeed);
        circmat = initializeCoords(num);
        if(initPositions!=null){
            positionAndRadius.AddRange(initPositions);
            for (int i = 0; i < initPositions.Count; i++)
            {
                circmat[i,0] = initPositions[i].x;
                circmat[i,1] = initPositions[i].y;
                circmat[i,2] = initPositions[i].z;
            }
        }
        FindAllRadii(circmat, initPositions!=null?initPositions.Count:0);
        return positionAndRadius;
    }

    static float[,] initializeCoords(int numell)
    {
        float[,] outmat = new float[numell, 3];
        for (int i = 0; i < numell; i++)
        {
            outmat[i, 0] = Random.Range(-width, width);
            outmat[i, 1] = Random.Range(-height, height);
            outmat[i, 2] = 0;
        }
        return outmat;
    }

    static bool FindAllRadii(float[,] cmat, int indx)
    {
        if (indx == num) return false;
        return FindAllRadii(FindNextRadius(cmat, indx), indx + 1);
    }

    static float[,] FindNextRadius(float[,] cmat, int indx, int overFlow = 0)
    {

        float[] distArr = new float[num - 1];
        float radius, r;
        for (int i = 0; i < num - 1; i++)
        {
            r = Mathf.Sqrt(Mathf.Pow(cmat[i, 0] - cmat[indx, 0], 2)
                    + Mathf.Pow(cmat[i, 1] - cmat[indx, 1], 2))
                        - cmat[i, 2];
            if (cmat[i, 2].Equals(0)) r = largeSize;
            distArr[i] = r;
        }

        radius = Mathf.Min(Mathf.Min(distArr), largeSize);

        if (radius > smallSize)
        {
            if (radius > smallSize && !IsTooClose(cmat, cmat[indx, 0], cmat[indx, 1], radius * 2, indx))
            {
                cmat[indx, 2] = radius;
                radius = radius * 2;
                positionAndRadius.Add(new Vector3(cmat[indx, 0], cmat[indx, 1], radius));
                return cmat;
            }
        }

        cmat[indx, 0] = Random.Range(-width, width);
        cmat[indx, 1] = Random.Range(-height, height);

        overFlow += 1;
        if(overFlow<1000)
            return FindNextRadius(cmat, indx, overFlow);
        else
            return cmat;

    }

    public static bool IsTooClose(float[,] cmat, float x, float y, float radius, int indx)
    {
        for (int i = 0; i < cmat.GetLength(0); i++)
        {
            if(i==indx) continue;
            else{
                Vector2 A = new Vector2(cmat[i,0],cmat[i,1]);
                Vector2 B = new Vector2(x,y);
                float distance = Vector2.Distance(A,B);
                float R1 = cmat[i,2];
                float R2 = radius;
            
                if (distance < ((R1+R2)*.5f))
                {
                    return true; // Too close to an existing point
                }
            }
        }
        return false; // Not too close to any existing points
    }


}

