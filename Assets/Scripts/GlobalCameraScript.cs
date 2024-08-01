using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalCameraScript : MonoBehaviour
{
    public GameObject player;


    public float sensitivity = 10f;
    public float moveSpeed = 10f;

    private float rotationY = 0f;
    private float rotationX = 0f;


    void Start()
    {
        transform.position = player.transform.position + new Vector3(0, 23, 0);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = player.transform.position + new Vector3(0, 23, 0);

        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

        // Update rotations
        rotationY += mouseX;
        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -90f, 90f);

        // Apply rotations
        transform.localRotation = Quaternion.Euler(rotationX, rotationY, 0f);

        // Get movement input
        float moveX = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        float moveZ = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;

        // Apply movement
        transform.Translate(new Vector3(moveX, 0, moveZ));
    }
}
