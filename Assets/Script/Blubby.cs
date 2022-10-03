using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blubby : MonoBehaviour
{
    public GameObject gameManager;
    
    public float velocity;
    public GameObject touch, secondaryCamera;
    public Camera myCamera;
    
    private static Rigidbody ballRigidbody;
    public static float angle = 0f;
    public static bool canFly;
    public static bool isBlubbyAlive = true;
    public static bool gameOver = false;
    bool cameraMove =false;
    public static int score;
    bool isGrounded, isSqueeze, isRotate, isLanded, isIdle;
    public static bool isJump;
    private Animator blubbyAnimation;

    bool canSound;
    bool isTouch;
    // canSqueeze will check if the blubby is squeezing or not for sound and for the first time it is true
    bool canSqueeze = true;

    // Start is called before the first frame update
    void Start()
    {
        touch.SetActive(true);
        score = -1;
        ballRigidbody = GetComponent<Rigidbody>();
        blubbyAnimation = GetComponent<Animator>();
        secondaryCamera.SetActive(false);
    }

    void Update()
    {
        if(canFly && !cameraMove){
            touch.SetActive(true);
        }else{
            touch.SetActive(false);
        }

        if (transform.position.y < 0)
        {
            touch.SetActive(false);
            gameOver = true;
            myCamera.transform.LookAt(this.transform.position);
        }
        else gameOver = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        blubbyAnimation.SetBool("IsGrounded", isGrounded);
        blubbyAnimation.SetBool("IsSqueeze", isSqueeze);
        blubbyAnimation.SetBool("IsJump", isJump);
        blubbyAnimation.SetBool("IsRotate", isRotate);
        blubbyAnimation.SetBool("IsLanded", isLanded);
        blubbyAnimation.SetBool("IsIdle", isIdle);

        if (transform.position.y < -50)
        {
            Time.timeScale = 0;
            isBlubbyAlive = false;
            Destroy(gameObject);
        }
        if (canFly && !cameraMove)
        {
            if(isTouch)
            {
                angle++;
                if (angle > 10)
                {
                    isSqueeze = true;
                    if(gameManager.GetComponent<GameManager>().canSound && canSqueeze){
                        canSqueeze = false;
                        gameManager.GetComponent<GameManager>().PlaySound("squeeze");
                    }
                }
            }
            else
            {
                if(angle>0)
                {
                    if(gameManager.GetComponent<GameManager>().canSound){
                    canSqueeze = true;
                    gameManager.GetComponent<GameManager>().PlaySound("jump");
                    }
                    isSqueeze = false;
                    isJump = true;

                    if (angle <10)
                    {
                        angle = 0f;
                        ballRigidbody.velocity = new Vector3(0f, 6f, 6f);
                    }
                    if (angle > 90f)
                    {
                        angle = 90f;
                    }
                    if (angle > 10)
                    {
                        ballRigidbody.velocity = new Vector3(0f, Mathf.Sin(angle / 2 * Mathf.Deg2Rad) * 2f, Mathf.Cos(angle / 2 * Mathf.Deg2Rad)) * velocity;
                    }
                    angle = 0f;
                }
            }
        }
        if (cameraMove)
        {
            if (myCamera.transform.position.z < transform.position.z -1f)
            {
                myCamera.transform.position = Vector3.Lerp(myCamera.transform.position, new Vector3(myCamera.transform.position.x, myCamera.transform.position.y, transform.position.z ), 3f * Time.deltaTime);
            }
            else
                cameraMove = false;
        }
    }

    public void ButtonDown()
    {
        isTouch = true;
    }
    public void ButtonUp()
    {
        isTouch = false;
    }
    void OnCollisionEnter(Collision collision)
    {
        // This checks if blubby is not on first ground
        if(collision.transform.position.z > 0 && gameManager.GetComponent<GameManager>().canSound)
        {
            gameManager.GetComponent<GameManager>().PlaySound("land");
        }
        cameraMove = true;
        isLanded = true;
        StartCoroutine(TimeForLand());
    }
    IEnumerator TimeForLand()
    {
        score ++;
        yield return new WaitForSeconds(.5f);
        isIdle = true;
        isRotate = false;
    }
    void OnCollisionStay(Collision collision)
    {
        isJump = false;
        isRotate = false;
        canFly = true;
    }
    void OnCollisionExit(Collision collision)
    {
        isSqueeze = false;
        canFly = false;
        cameraMove = false;
        isJump = true;
        StartCoroutine(IsInAir());
    }
    IEnumerator IsInAir()
    {
        yield return new WaitForSeconds(.2f);
        isJump = false;
        isRotate = true;
        isIdle = false;
        isLanded = false;
    }
}
