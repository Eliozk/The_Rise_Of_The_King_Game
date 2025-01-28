using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeScaler : MonoBehaviour
{
    // Public variables to control the scale of the object along each axis
    public float xScale;  // Scale factor for the x-axis
    public float yScale;  // Scale factor for the y-axis
    public float zScale;  // Scale factor for the z-axis

    // Start is called before the first frame update
    public void Start()
    {
        // Set the local scale of the object based on the public scale values
        transform.localScale = new Vector3(xScale, yScale, zScale);
    }
}
