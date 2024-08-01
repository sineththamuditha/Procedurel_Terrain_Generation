using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    public GameObject mapPreview;
    public GameObject mapGenerator;

    public GameObject thirdPersonController;
    public GameObject globalCamera;

    public GameObject uiElements;
    public GameObject mapPreviewCamera;

    bool inPreviewMode;
    bool inGeneratorMode;

    void Start()
    {
        mapPreview.SetActive(false);
        mapGenerator.SetActive(true);

        thirdPersonController.SetActive(true);
        globalCamera.SetActive(false);

        uiElements.SetActive(false);
        mapPreviewCamera.SetActive(false);

        inGeneratorMode = true;
        inPreviewMode = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G) && inGeneratorMode)
        {
            thirdPersonController.SetActive(!thirdPersonController.activeSelf);
            globalCamera.SetActive(!globalCamera.activeSelf);
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            
            mapGenerator.SetActive(!mapGenerator.activeSelf);
            mapPreview.SetActive(!mapPreview.activeSelf);

            Debug.Log(mapPreview.activeSelf);

            if (Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.None;
            } else
            {
                Cursor.lockState = CursorLockMode.Locked;
            }

            uiElements.SetActive(!uiElements.activeSelf);

            if (inGeneratorMode)
            {
                thirdPersonController.SetActive(false);
                globalCamera.SetActive(false);

                mapPreviewCamera.SetActive(true);

                inPreviewMode = true;
                inGeneratorMode = false;

                return;
            }

           if (inPreviewMode)
            {
                thirdPersonController.SetActive(true);
                globalCamera.SetActive(false);

                mapPreviewCamera.SetActive(false);

                inGeneratorMode = true;
                inPreviewMode = false;
            }
        }
    }
}
