using System.Collections.Generic;
using UnityEngine;

public class TerrainChunk
{

    const float colliderGenerationDistanceThreshold = 5;
    public event System.Action<TerrainChunk, bool> onVisibilityChanged;
    public Vector2 coord;

    GameObject meshObject;
    Vector2 sampleCentre;
    Bounds bounds;

    MeshRenderer meshRenderer;
    MeshFilter meshFilter;
    MeshCollider meshCollider;

    LODInfo[] detailLevels;
    LODMesh[] lodMeshes;
    int colliderLODIndex;

    HeightMap heightMap;
    bool heightMapReceived;
    int previousLODIndex = -1;
    bool hasSetCollider;
    float maxViewDst;

    HeightMapSettings heightMapSettings;
    MeshSettings meshSettings;
    Transform viewer;

    GameObject treePrefab;
    public float treePlacementProbability = 0.01f; 
    public float minTreeHeight = 5f; 
    public float maxTreeHeight = 20f; 

    List<GameObject> treeObjects = new List<GameObject>();

    GameObject waterPlane;

    public TerrainChunk(Vector2 coord, HeightMapSettings heightMapSettings, MeshSettings meshSettings, LODInfo[] detailLevels, int colliderLODIndex, Transform parent, Transform viewer, Material material, GameObject treePrefab, Material waterMaterial, TextureData textureData)
    {
        this.coord = coord;
        this.detailLevels = detailLevels;
        this.colliderLODIndex = colliderLODIndex;
        this.heightMapSettings = heightMapSettings;
        this.meshSettings = meshSettings;
        this.viewer = viewer;
        this.treePrefab = treePrefab;

        sampleCentre = coord * meshSettings.meshWorldSize / meshSettings.meshScale;
        Vector2 position = coord * meshSettings.meshWorldSize;
        bounds = new Bounds(position, Vector2.one * meshSettings.meshWorldSize);


        meshObject = new GameObject("Terrain Chunk");
        meshRenderer = meshObject.AddComponent<MeshRenderer>();
        meshFilter = meshObject.AddComponent<MeshFilter>();
        meshCollider = meshObject.AddComponent<MeshCollider>();
        meshRenderer.material = material;

        meshObject.transform.position = new Vector3(position.x, 0, position.y);
        meshObject.transform.parent = parent;
        SetVisible(false);

        lodMeshes = new LODMesh[detailLevels.Length];
        for (int i = 0; i < detailLevels.Length; i++)
        {
            lodMeshes[i] = new LODMesh(detailLevels[i].lod, treePrefab, meshObject);
            lodMeshes[i].updateCallback += UpdateTerrainChunk;
            if (i == colliderLODIndex)
            {
                lodMeshes[i].updateCallback += UpdateCollisionMesh;
            }
        }

        maxViewDst = detailLevels[detailLevels.Length - 1].visibleDstThreshold;

        // Adding water
        if (waterPlane == null)
        {
            waterPlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
            waterPlane.GetComponent<MeshRenderer>().material = waterMaterial;
        }

        Debug.Log(meshObject.transform);
        waterPlane.transform.parent = meshObject.transform;
        waterPlane.transform.localPosition = meshObject.transform.localPosition + new Vector3(0, textureData.layers[1].startHeight + 0.9f, 0);
        waterPlane.transform.localScale = new Vector3(((float)meshSettings.numVertsPerLine)/5, 1, ((float)meshSettings.numVertsPerLine)/5);
        waterPlane.SetActive(false);

    }

    public void Load()
    {
        ThreadedDataRequester.RequestData(() => HeightMapGenerator.GenerateHeightMap(meshSettings.numVertsPerLine, meshSettings.numVertsPerLine, heightMapSettings, sampleCentre), OnHeightMapReceived);
    }



    void OnHeightMapReceived(object heightMapObject)
    {
        this.heightMap = (HeightMap)heightMapObject;
        heightMapReceived = true;

        UpdateTerrainChunk();
    }

    Vector2 viewerPosition
    {
        get
        {
            return new Vector2(viewer.position.x, viewer.position.z);
        }
    }


    public void UpdateTerrainChunk()
    {
        if (heightMapReceived)
        {
            float viewerDstFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(viewerPosition));

            bool wasVisible = IsVisible();
            bool visible = viewerDstFromNearestEdge <= maxViewDst;

            if (visible)
            {
                int lodIndex = 0;

                for (int i = 0; i < detailLevels.Length - 1; i++)
                {
                    if (viewerDstFromNearestEdge > detailLevels[i].visibleDstThreshold)
                    {
                        lodIndex = i + 1;
                    }
                    else
                    {
                        break;
                    }
                }

                if (lodIndex != previousLODIndex)
                {
                    LODMesh lodMesh = lodMeshes[lodIndex];
                    if (lodMesh.hasMesh)
                    {
                        previousLODIndex = lodIndex;
                        meshFilter.mesh = lodMesh.mesh;
                    }
                    else if (!lodMesh.hasRequestedMesh)
                    {
                        lodMesh.RequestMesh(heightMap, meshSettings, treePrefab, treePlacementProbability, minTreeHeight, maxTreeHeight);
                    }
                }


            }

            if (wasVisible != visible)
            {

                SetVisible(visible);
                waterPlane.SetActive(visible);
                if (onVisibilityChanged != null)
                {
                    onVisibilityChanged(this, visible);
                }
            }
        }
    }

    public void UpdateCollisionMesh()
    {
        if (!hasSetCollider)
        {
            float sqrDstFromViewerToEdge = bounds.SqrDistance(viewerPosition);

            if (sqrDstFromViewerToEdge < detailLevels[colliderLODIndex].sqrVisibleDstThreshold)
            {
                if (!lodMeshes[colliderLODIndex].hasRequestedMesh)
                {
                    lodMeshes[colliderLODIndex].RequestMesh(heightMap, meshSettings, treePrefab, treePlacementProbability, minTreeHeight, maxTreeHeight); ;
                }
            }

            if (sqrDstFromViewerToEdge < colliderGenerationDistanceThreshold * colliderGenerationDistanceThreshold)
            {
                if (lodMeshes[colliderLODIndex].hasMesh)
                {
                    meshCollider.sharedMesh = lodMeshes[colliderLODIndex].mesh;
                    hasSetCollider = true;
                }
            }
        }
    }

    public void SetVisible(bool visible)
    {
        meshObject.SetActive(visible);
    }

    public bool IsVisible()
    {
        return meshObject.activeSelf;
    }

}

class LODMesh
{

    public Mesh mesh;
    public bool hasRequestedMesh;
    public bool hasMesh;
    int lod;
    public event System.Action updateCallback;

    GameObject treePrefab;
    List<GameObject> generatedTrees = new List<GameObject>();

    GameObject parent;

    public LODMesh(int lod, GameObject treePrefab, GameObject parent)
    {
        this.lod = lod;
        this.treePrefab = treePrefab;
        this.parent = parent;
    }

    void OnMeshDataReceived(object meshDataObject)
    {
        MeshData meshData = (MeshData)meshDataObject;
        mesh = meshData.CreateMesh();
        hasMesh = true;

        generatedTrees = meshData.placeTrees(treePrefab, parent.transform.position, parent.transform);

        updateCallback();
    }

    void clearTrees()
    {
        foreach (GameObject tree in generatedTrees)
        {
            Object.DestroyImmediate(tree);
        }

        generatedTrees.Clear();
    }

    public void RequestMesh(HeightMap heightMap, MeshSettings meshSettings, GameObject treePrefab, float treePlacementProbability, float minTreeHeight, float maxTreeHeight)
    {
        hasRequestedMesh = true;
        ThreadedDataRequester.RequestData(() => MeshGenerator.GenerateTerrainMesh(heightMap.values, meshSettings, lod, treePlacementProbability, minTreeHeight, maxTreeHeight), OnMeshDataReceived);
    }

}