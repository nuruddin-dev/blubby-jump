using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground : MonoBehaviour
{
    // To move the ground to another random position we used predefined positions coming from GameManager
    GameObject gameManager;
    // Last array position is tracking to set the current ground to the next array position
    public static int lastArrayPos = 0;

    void Start()
    {
        gameManager = GameObject.Find("Game Manager");
    }
    
    // When blubby departs from one ground then after 3 seconds the ground will be moved to other position
    void OnCollisionExit(Collision collision)
    {
        StartCoroutine(MoveTheGround());
    }

    IEnumerator MoveTheGround()
    {
        yield return new WaitForSeconds(3f);
        transform.position = new Vector3(transform.position.x, transform.position.y, gameManager.GetComponent<GameManager>().zPositions[lastArrayPos]);
        lastArrayPos++;
    }

}
