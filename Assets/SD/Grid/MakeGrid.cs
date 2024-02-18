using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class MakeGrid : SDRenderChainLink
{
    public GameObject quadPrefab; // Assign a Quad prefab with a MeshRenderer component
    public int gridScale = 10;
    private GameObject[,] gridArray;
    public Camera textureCam;
    int gridIndexX = 0;
    int gridIndexY = 0;
    public string saverLocation;
    public string imageName;

    public int resolution = 512;

    public SDRenderChainLinkSave saver;

    public bool render = true;

    void Start()
    {
        // GenerateGrid();
        RunUnityFunction("");
    }

    public override void RunUnityFunction(string image)
    {
        // if(image.Length != 0){
        if (render)
        {
            gridIndexX++;
            if (gridIndexX >= gridScale)
            {
                gridIndexX = 0;
                gridIndexY++;
                if (gridIndexY >= gridScale)
                {
                    gridIndexY = 0;
                    render = false;
                }
            }
            float scale = 1.0f / (float)gridScale;
            scale *= .5f;
            textureCam.orthographicSize = scale;
            textureCam.transform.position = new Vector3(gridIndexX + scale - .5f, gridIndexY + scale - .5f, -10);
            // UpdateCellTexture(gridIndexX, gridIndexY, tex);
            saver.location = saverLocation + imageName + gridIndexX + "_" + gridIndexY + ".png";

            Texture2D tex = SDRenderUtils.Capture(textureCam, (int)resolution, (int)resolution);
            string outImage = Convert.ToBase64String(tex.EncodeToPNG());

            foreach (SDRenderChainLink l in link)
            {
                l.RunUnityFunction("");
            }
        }

    }

    void GenerateGrid()
    {
        gridArray = new GameObject[gridScale, gridScale];
        // gridIndex = new int[gridScale, gridScale];

        for (int x = 0; x < gridScale; x++)
        {
            for (int y = 0; y < gridScale; y++)
            {
                float scale = 1.0f / (gridScale);
                GameObject gridQuad = Instantiate(quadPrefab, new Vector3(x - scale * .5f, y - scale * .5f, 0), Quaternion.identity);

                gridQuad.transform.localScale = new Vector3(scale, scale, scale);
                gridQuad.name = $"Quad_{x}_{y}";
                gridQuad.transform.parent = this.transform; // Set the grid parent for better hierarchy organization

                // Placeholder for texture assignment
                // You would replace or modify this with your method of applying textures
                gridQuad.GetComponent<MeshRenderer>().material = new Material(Shader.Find("Texture"));

                gridArray[x, y] = gridQuad;
            }
        }
    }

    // Example method to update a single cell's texture
    public void UpdateCellTexture(int x, int y, Texture2D newTexture)
    {
        if (x < 0 || x >= gridScale || y < 0 || y >= gridScale) return; // Bounds check

        GameObject cell = gridArray[x, y];
        if (cell != null)
        {
            MeshRenderer renderer = cell.GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                renderer.material.mainTexture = newTexture;
            }
        }
    }
}
