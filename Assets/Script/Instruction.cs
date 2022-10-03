using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Instruction : MonoBehaviour
{
    public GameObject blubby;

    // Update is called once per frame
    void Update()
    {
        if(blubby.transform.position.z > 0)
        {
            gameObject.SetActive(false);
        }
    }
}
