using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraMovement : MonoBehaviour
{
    public Transform[] cameras;
    int selectedCamera;
    public Placement placement;

    float originalSpeed;
    public float rotationSpeed;

    float rotationX;
    float rotationY;

    float xRotation;
    float yRotation;

    public float initialDistance;
    public float sensitivity = 5;
    public float maxZoom = 140;
    public List<Material> skyboxes;
    public bool zooming;
    private bool moved;

    private Vector2 startPos;

    void Start()
    {
        //PlayerPrefs.SetString("previousScene", SceneManager.GetActiveScene().name);
        /*
        if (level1)
        {
            
        }

        if (level2)
        {
            PlayerPrefs.SetString("previousScene", SceneManager.GetActiveScene().name);
        }
        */
        rotationSpeed = PlayerPrefs.GetFloat("rotationSpeed");
        sensitivity = PlayerPrefs.GetFloat("zoomSensitivity");
        originalSpeed = rotationSpeed;
    }

    // Update is called once per frame
    void Update()
    {

        CheckSkybox();
        if (placement != null)
        {
            if (!placement.deletingObject)
            //for level 1
            {
                SwitchCamera();
                ZoomCamera();
                TemporaryCamera();
            }
        }
        else 
        {

            //level 2
            
                SwitchCamera();
                TemporaryCamera();
                ZoomCamera();
        }
            //EnhancedCamera();
    }

    void CheckSkybox()
    //sets the skybox of the game level based on the int
    //selects a background based on the index, array of backgrounds is set in the variables
    {
        RenderSettings.skybox = skyboxes[PlayerPrefs.GetInt("backgroundIndex")];
    }

    void EnhancedCamera()
    //not used, intended purpose?
    {
        transform.LookAt(cameras[selectedCamera]);
        //transform.Translate(Vector3.up * Input.GetAxisRaw("Vertical") * rotationSpeed * Time.deltaTime);
        transform.Translate(Vector3.right * Input.GetAxisRaw("Horizontal") * rotationSpeed * Time.deltaTime);
        //float moveX = Input.GetAxis("Horizontal") * rotationSpeed * Time.deltaTime;
        float moveY = Input.GetAxis("Vertical") * rotationSpeed * Time.deltaTime;

        xRotation -= moveY;
        xRotation = Mathf.Clamp(xRotation, -10f, 10f);

        transform.localRotation = Quaternion.Euler(xRotation * rotationSpeed, 0f, 0f);
    }

    void TemporaryCamera()
    //switches the camera's rotation based on different points in the scene
    //allows movement of the camera getting it from input, applying the vector direction then set it across the delta time
    {
        transform.LookAt(cameras[selectedCamera]);
#if UNITY_STANDALONE
        transform.Translate(Vector3.up * Input.GetAxisRaw("Vertical") * rotationSpeed * Time.deltaTime);
        transform.Translate(Vector3.right * Input.GetAxisRaw("Horizontal") * rotationSpeed * Time.deltaTime);
#endif
#if UNITY_ANDROID
        if (Input.touchCount == 1)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                startPos = Input.GetTouch(0).position;
            }
            else if (Input.GetTouch(0).phase == TouchPhase.Moved)
            //if the finger has moved
            //checks the difference between the intial touch position of the finger to the new finger position
            {
                Vector2 swipeDelta = Input.GetTouch(0).position - startPos;
                float rotationAmountX = swipeDelta.x * Time.deltaTime;
                float rotationAmountY = swipeDelta.y * Time.deltaTime;
                transform.RotateAround(cameras[0].position, Vector3.up, rotationAmountX);
                transform.RotateAround(cameras[0].position, Vector3.forward, rotationAmountY);
            }
        }
#endif
        if (Input.GetKey(KeyCode.LeftShift))
        {
            rotationSpeed = originalSpeed * 2;
        }
        else
        {
            rotationSpeed = originalSpeed;
        }
            
        
    }

    void ZoomCamera() 
    {
        if (Input.touchCount == 2)
        {
            Vector3 currentRotation = transform.eulerAngles;
            zooming = true;
            moved = true;
            if (Input.GetTouch(0).phase == TouchPhase.Began || Input.GetTouch(1).phase == TouchPhase.Began)
            {
                initialDistance = Vector2.Distance(Input.GetTouch(0).position, Input.GetTouch(1).position);
            }
            else if (Input.GetTouch(0).phase == TouchPhase.Moved || Input.GetTouch(1).phase == TouchPhase.Moved)
            {
                transform.eulerAngles = currentRotation;
                float currentDistance = Vector2.Distance(Input.GetTouch(0).position, Input.GetTouch(1).position);
                float deltaDistance = currentDistance - initialDistance;

                float zoomAmount = deltaDistance * sensitivity * Time.deltaTime;
                float zoom = Mathf.Clamp(transform.position.x + zoomAmount, -maxZoom, maxZoom);

                Vector3 newPosition = transform.position;
                newPosition.x = zoom;
                transform.position = newPosition;

                ClipCheck();
            }
        }
        else 
        {
            if (!moved)
            {
                transform.position = Vector3.MoveTowards(transform.position, transform.forward, 100 * Time.deltaTime);
                if (Vector3.Distance(transform.position, cameras[0].position) < 60)
                {
                    moved = true;
                }
            }
            
            zooming = false;
        }
    }

    //void RotateCamera()
    //{
    //    float moveX = Input.GetAxisRaw("Horizontal") * rotationSpeed;
    //    float moveY = Input.GetAxisRaw("Vertical") * rotationSpeed;

    //    rotationX += moveX;
    //    rotationY += moveY;
    //    rotationX = Mathf.Clamp(rotationX, -40, 40);

    //    transform.localEulerAngles = new Vector3(rotationX, rotationY, 0);

    //    transform.position = target.position - transform.forward;
    //}


    void SwitchCamera()
    //switches camera between different transforms in the scene which provides slightly altered view of the game
    //not sure what is the goal but seems to switch intended view of the level, but can already customise view
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (selectedCamera > 0)
            {
                selectedCamera--;
            }
            else
            {
                selectedCamera = cameras.Length - 1;
            }
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (selectedCamera < cameras.Length - 1)
            {
                selectedCamera++;
            }
            else
            {
                selectedCamera = 0;
            }
        }
    }

    void ClipCheck()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        if (Physics.SphereCast(ray, 1, out RaycastHit hit, maxZoom))
        {
            if (hit.distance < 60f)
            {
                transform.position = new Vector3(hit.point.x - 60f, hit.point.y, hit.point.z);
            }
        }
    }
}
