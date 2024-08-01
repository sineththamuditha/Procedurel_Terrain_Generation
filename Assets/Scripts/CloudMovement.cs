using UnityEngine;

public class CloudMovement : MonoBehaviour
{
    public Material cloudMaterial;
    public float cloudSpeed = 1.0f;
    public Transform viewer;  // The viewer's transform (e.g., the main camera)
    public float heightAboveTerrain = 30.0f;  // Height above the terrain

    void Update()
    {
        // Update the cloud movement over time
        float time = Time.time * cloudSpeed;
       
        // Keep the clouds above the viewer
        Vector3 newPosition = viewer.position;
        newPosition.y = heightAboveTerrain;
        transform.position = newPosition;
    }
}
