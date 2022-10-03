using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewMat : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        transform.Rotate(10*Time.deltaTime,10*Time.deltaTime,10*Time.deltaTime); //rotates 10 degrees per second around each axis
    }
}
