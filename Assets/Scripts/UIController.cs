using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public MapPreview mapPreview;
    public GameObject cloudLayer;

    public Button resetButton;
    public Button cloudToggleButton;

    public Slider levelOfDetailSlider;
    public Slider noiseScaleSlider;
    public Slider octaveSlider;
    public Slider persistenceSlider;
    public Slider lacunaritySlider;
    public Slider seedSlider;
    public Slider terrainHeightSlider;
    public Slider offsetXSlider;
    public Slider offsetYSlider;
    public Slider treeProbability;
    public Slider minTreeHeight;
    public Slider maxTreeHeight;

    private bool mapPreviewView = true;
    private bool valueChanged = false;

    void Start()
    {
        mapPreview.DrawMapInEditor();

        resetButton.onClick.AddListener(delegate { resetValues(); });
        cloudToggleButton.onClick.AddListener(delegate {
            cloudLayer.SetActive(!cloudLayer.activeSelf);
        });

        // Initialize Terrain Related Sliders
        initializeLevelOfDetailSlider();
        initiliazeNoiseScaleSlider();
        initializeOctaveSlider();
        initializePersistenceSlider();
        initializeLacunaritySlider();
        initializeSeedSlider();
        initializeTerrainHeightSlider();
        initializeOffsetXSlider();
        initializeOffsetYSlider();

        // Initialize Tree Sliders
        initializeTreeProbabilitySlider();
        initializeMinTreeHeightSlider();
        initializeMaxTreeHeightSlider();
    }

    // Update is called once per frame
    void Update()
    {
        if (mapPreviewView && valueChanged)
        {
            valueChanged = false;
            mapPreview.DrawMapInEditor();
        }
    }


    void resetValues()
    {
        levelOfDetailSlider.value = 0;
        mapPreview.editorPreviewLOD = (int)levelOfDetailSlider.value;

        noiseScaleSlider.value = 50;
        mapPreview.heightMapSettings.noiseSettings.scale = noiseScaleSlider.value;

        octaveSlider.value = 6;
        mapPreview.heightMapSettings.noiseSettings.octaves = (int)octaveSlider.value;

        persistenceSlider.value = 0.547f;
        mapPreview.heightMapSettings.noiseSettings.persistance = persistenceSlider.value;


        lacunaritySlider.value = 2;
        mapPreview.heightMapSettings.noiseSettings.lacunarity = lacunaritySlider.value;

        seedSlider.value = 0;
        mapPreview.heightMapSettings.noiseSettings.seed = (int)seedSlider.value;
       
        terrainHeightSlider.value = 0;
        mapPreview.heightMapSettings.heightMultiplier = terrainHeightSlider.value;
        
        offsetXSlider.value = 0;
        mapPreview.heightMapSettings.noiseSettings.offset.x = offsetXSlider.value;
        
        offsetYSlider.value = 0;
        mapPreview.heightMapSettings.noiseSettings.offset.y = offsetYSlider.value;
        
        treeProbability.value = 0.1f; 
        mapPreview.treePlacementProbability = treeProbability.value;
        
        minTreeHeight.value = 10.0f;
        mapPreview.minTreeHeight = minTreeHeight.value;
        
        maxTreeHeight.value = 20.0f;
        mapPreview.maxTreeHeight = minTreeHeight.value;

        valueChanged = true;
    }

    void initializeLevelOfDetailSlider()
    {
        levelOfDetailSlider.value = 0;
        levelOfDetailSlider.maxValue = 4;
        levelOfDetailSlider.minValue = 0;

        levelOfDetailSlider.onValueChanged.AddListener(delegate {
            mapPreview.editorPreviewLOD = (int)levelOfDetailSlider.value;
            valueChanged = true;
        });
    }

    void initiliazeNoiseScaleSlider()
    {
        noiseScaleSlider.value = 50;
        noiseScaleSlider.minValue = 0;
        noiseScaleSlider.maxValue = 100;

        noiseScaleSlider.onValueChanged.AddListener(delegate {
            mapPreview.heightMapSettings.noiseSettings.scale = noiseScaleSlider.value;
            valueChanged = true;
        });
    }

    void initializeOctaveSlider()
    {
        octaveSlider.value = 6;
        octaveSlider.minValue = 1;
        octaveSlider.maxValue = 15;

        octaveSlider.onValueChanged.AddListener(delegate {
            mapPreview.heightMapSettings.noiseSettings.octaves = (int)octaveSlider.value;
            valueChanged = true;
        });
    }

    void initializePersistenceSlider()
    {
        persistenceSlider.value = 0.547f;
        persistenceSlider.minValue = 0.0f;
        persistenceSlider.maxValue = 1.0f;

        persistenceSlider.onValueChanged.AddListener(delegate {
            mapPreview.heightMapSettings.noiseSettings.persistance = persistenceSlider.value;
            valueChanged = true;
        });
    }

    void initializeLacunaritySlider()
    {
        lacunaritySlider.value = 2;
        lacunaritySlider.minValue = 0;
        lacunaritySlider.maxValue = 10;

        lacunaritySlider.onValueChanged.AddListener(delegate {
            mapPreview.heightMapSettings.noiseSettings.lacunarity = lacunaritySlider.value;
            valueChanged = true;
        });
    }

    void initializeSeedSlider()
    {
        seedSlider.value = 0;
        seedSlider.minValue = 0;
        seedSlider.maxValue = 100;

        seedSlider.onValueChanged.AddListener(delegate {
            mapPreview.heightMapSettings.noiseSettings.seed = (int)seedSlider.value;
            valueChanged = true;
        });
    }

    void initializeTerrainHeightSlider()
    {
        terrainHeightSlider.value = 0;
        terrainHeightSlider.minValue = 30;
        terrainHeightSlider.maxValue = 100;

        terrainHeightSlider.onValueChanged.AddListener(delegate {
            mapPreview.heightMapSettings.heightMultiplier = terrainHeightSlider.value;
            valueChanged = true;
        });
    }

    void initializeOffsetXSlider()
    {
        offsetXSlider.value = 0;
        offsetXSlider.minValue = 0;
        offsetXSlider.maxValue = 100;

        offsetXSlider.onValueChanged.AddListener(delegate { 
            mapPreview.heightMapSettings.noiseSettings.offset.x = offsetXSlider.value;
            valueChanged = true;
        });

    }

    void initializeOffsetYSlider()
    {
        offsetYSlider.value = 0;
        offsetYSlider.minValue = 0;
        offsetYSlider.maxValue = 100;

        offsetYSlider.onValueChanged.AddListener(delegate {
            mapPreview.heightMapSettings.noiseSettings.offset.y = offsetYSlider.value;
            valueChanged = true;
        });
    }

    void initializeTreeProbabilitySlider()
    {
        treeProbability.value = 0.1f;
        treeProbability.maxValue = 1.0f;
        treeProbability.minValue = 0.0f;

        treeProbability.onValueChanged.AddListener(delegate {
            mapPreview.treePlacementProbability = treeProbability.value;
            valueChanged = true;
        });
    }

    void initializeMinTreeHeightSlider()
    {
        minTreeHeight.value = 10.0f;
        minTreeHeight.maxValue = 15.0f;
        minTreeHeight.minValue = 5.0f;

        minTreeHeight.onValueChanged.AddListener(delegate {
            mapPreview.minTreeHeight = minTreeHeight.value;
            valueChanged = true;
        });
    }

    void initializeMaxTreeHeightSlider()
    {
        maxTreeHeight.value = 20.0f;
        maxTreeHeight.maxValue = 25.0f;
        maxTreeHeight.minValue = 20.0f;

        maxTreeHeight.onValueChanged.AddListener(delegate {
            mapPreview.maxTreeHeight = maxTreeHeight.value;
            valueChanged = true;
        });
    }
}
