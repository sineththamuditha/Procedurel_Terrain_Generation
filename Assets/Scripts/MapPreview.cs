using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

public class MapPreview : MonoBehaviour
{

    public Renderer textureRender;
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;


    public enum DrawMode { NoiseMap, Mesh, FalloffMap };
    public DrawMode drawMode;

    public MeshSettings meshSettings;
    public HeightMapSettings heightMapSettings;
    public TextureData textureData;

    public Material terrainMaterial;



    [Range(0, MeshSettings.numSupportedLODs - 1)]
    public int editorPreviewLOD;
    public bool autoUpdate;

    public GameObject treePrefab;
    [Range(0, 1)]
    public float treePlacementProbability = 0.1f; // Probability of a tree spawning
    [Range (5, 15)]
    public float minTreeHeight = 20f; // Minimum height for tree placement
    [Range (20, 25)]
    public float maxTreeHeight = 50f; // Maximum height for tree placement

    List<GameObject> generatedTrees = new List<GameObject>();

    public Material waterMaterial;

    GameObject waterPlane;

    public void DrawMapInEditor()
    {
        textureData.ApplyToMaterial(terrainMaterial);
        textureData.UpdateMeshHeights(terrainMaterial, heightMapSettings.minHeight, heightMapSettings.maxHeight);
        HeightMap heightMap = HeightMapGenerator.GenerateHeightMap(meshSettings.numVertsPerLine, meshSettings.numVertsPerLine, heightMapSettings, Vector2.zero);

        if (waterPlane == null )
        {
            waterPlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
            waterPlane.transform.parent = meshFilter.transform;
        }

        waterPlane.GetComponent<MeshRenderer>().material = waterMaterial;
        waterPlane.transform.position = new Vector3(0, textureData.layers[1].startHeight + 0.9f, 0);
        waterPlane.transform.localScale = new Vector3(((float)meshSettings.numVertsPerLine)/10, 1 , ((float)meshSettings.numVertsPerLine) / 10);

        if (drawMode == DrawMode.NoiseMap)
        {
            DrawTexture(TextureGenerator.TextureFromHeightMap(heightMap));
        }
        else if (drawMode == DrawMode.Mesh)
        {
            DrawMesh(MeshGenerator.GenerateTerrainMesh(heightMap.values, meshSettings, editorPreviewLOD, treePlacementProbability, minTreeHeight, maxTreeHeight));
        }
        else if (drawMode == DrawMode.FalloffMap)
        {
            DrawTexture(TextureGenerator.TextureFromHeightMap(new HeightMap(FalloffGenerator.GenerateFalloffMap(meshSettings.numVertsPerLine), 0, 1)));
        }
    }





    public void DrawTexture(Texture2D texture)
    {
        textureRender.sharedMaterial.mainTexture = texture;
        textureRender.transform.localScale = new Vector3(texture.width, 1, texture.height) / 10f;

        textureRender.gameObject.SetActive(true);
        meshFilter.gameObject.SetActive(false);
    }

    public void DrawMesh(MeshData meshData)
    {
        meshFilter.sharedMesh = meshData.CreateMesh();

        Debug.Log(textureData.layers[1].startHeight);


        clearTrees();
        generatedTrees = meshData.placeTrees(treePrefab, meshRenderer.transform.position, meshFilter.transform);

        textureRender.gameObject.SetActive(false);
        meshFilter.gameObject.SetActive(true);
    }

    void clearTrees()
    {
        foreach (GameObject tree in generatedTrees)
        {
            Object.DestroyImmediate(tree);
        }

        generatedTrees.Clear();
    }



    void OnValuesUpdated()
    {
        if (!Application.isPlaying)
        {
            DrawMapInEditor();
        }
    }

    void OnTextureValuesUpdated()
    {
        textureData.ApplyToMaterial(terrainMaterial);
    }

    void OnValidate()
    {

        if (meshSettings != null)
        {
            meshSettings.OnValuesUpdated -= OnValuesUpdated;
            meshSettings.OnValuesUpdated += OnValuesUpdated;
        }
        if (heightMapSettings != null)
        {
            heightMapSettings.OnValuesUpdated -= OnValuesUpdated;
            heightMapSettings.OnValuesUpdated += OnValuesUpdated;
        }
        if (textureData != null)
        {
            textureData.OnValuesUpdated -= OnTextureValuesUpdated;
            textureData.OnValuesUpdated += OnTextureValuesUpdated;
        }

    }

}
