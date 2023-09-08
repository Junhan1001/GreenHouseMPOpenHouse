using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CameraMovement : MonoBehaviour
{
    public Transform[] cameras;
    int selectedCamera;
    public Placement1 placement;
    public Hover hover;
    public HoverGroup hoverGrp;
    public Connection connection;

    public Sprite unlockCam, lockCam;
    public Image cameraLockBtn;

    public bool allowRotation = true;
    public bool allowZoom = true;

    float originalSpeed;
    public float rotationSpeed;

    public float scrollFloor, scrollCeiling;

    private float initialDistance;
    public float sensitivity = 5;
    public float maxZoom = 140;
    public float zoomStopDistance = 60;
    private Material[] skyboxes;
    public bool zooming;
    public bool moved;

    private Vector2 startPos;
    [SerializeField]
    private float zoomAmount;
    private float deltaDistance;

    [SerializeField]
    private float maxRotationX = 30f;
    [SerializeField]
    private float minRotationX = 340f;
    [SerializeField]
    private float maxRotationY = 150f;
    [SerializeField]
    private float minRotationY = 30f;

    public float cameraDist;

    private bool isPrelevel = false;

    private void OnEnable()
    {
        //if (SceneManager.GetActiveScene().buildIndex == 0)
        if (SceneManager.GetActiveScene().name == "PreviewLevel")
        {
            isPrelevel = true;
        }
    }

    void Start()
    {
        skyboxes = Resources.LoadAll<Material>("Skyboxes");
        rotationSpeed = PlayerPrefs.GetFloat("rotationSpeed");
        sensitivity = PlayerPrefs.GetFloat("zoomSensitivity");
        originalSpeed = rotationSpeed;
        zoomAmount = Camera.main.fieldOfView;


        if (!isPrelevel)
        {
            cameraDist = Vector3.Distance(cameras[0].transform.position, transform.position);
            Camera.main.fieldOfView = 40f;
            zoomAmount = Camera.main.fieldOfView;
            StartCoroutine(ZoomCameraCoroutine());
        }
    }

    private IEnumerator ZoomCameraCoroutine()
    {
        while (true)
        {
            Camera.main.fieldOfView -= sensitivity * 10f * Time.deltaTime;
            if (Camera.main.fieldOfView <= 25)
            {
                Camera.main.fieldOfView = 25;
                zoomAmount = Camera.main.fieldOfView;
                yield break;
            }
            yield return null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        CheckSkybox();
        if (isPrelevel)
        {
            CameraRotation();
            ZoomCamera();
            return;
        }
        if (placement != null)
        {
            CameraScroll();
            ZoomCamera();
        }
        else
        //level 2
        {
            CameraScroll();
            //SwitchCamera();
            //CameraRotation();
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

    void CameraRotation()
    //switches the camera's rotation based on different points in the scene
    //allows movement of the camera getting it from input, applying the vector direction then set it across the delta time
    {
        if (!allowRotation)
            return;
        if (zooming) 
        {
            return;
        }

#if UNITY_STANDALONE
        transform.LookAt(cameras[selectedCamera]);
        //transform.Translate(Vector3.up * Input.GetAxisRaw("Vertical") * rotationSpeed * Time.deltaTime);
        //transform.Translate(Vector3.right * Input.GetAxisRaw("Horizontal") * rotationSpeed * Time.deltaTime);
        float rotationAmountY = -Input.GetAxisRaw("Horizontal") * rotationSpeed * Time.deltaTime;
        float rotationAmountX = Input.GetAxisRaw("Vertical") * rotationSpeed * Time.deltaTime;
        float currentEulerAngleY = transform.rotation.eulerAngles.y;
        float currentEulerAngleX = transform.rotation.eulerAngles.x;
        if (isPrelevel)
        {
            if (rotationAmountX != 0)
            {
                if ((currentEulerAngleX + rotationAmountX) > maxRotationX + 30f && (currentEulerAngleX + rotationAmountX) < 180)
                {
                    rotationAmountX = 0;
                    //rotationAmountX = maxRotationX - currentEulerAngleX;
                }
                else if ((currentEulerAngleX + rotationAmountX) < minRotationX && (currentEulerAngleX + rotationAmountX > 180))
                {
                    rotationAmountX = 0;
                    //rotationAmountX = maxRotationX - currentEulerAngleX;
                }
            }
            transform.RotateAround(cameras[0].position, transform.up, rotationAmountY);
            transform.RotateAround(cameras[0].position, transform.right, rotationAmountX);
            return;
        }
        if (rotationAmountY != 0)
        {
            if ((currentEulerAngleY + rotationAmountY) > maxRotationY)
            {
                //rotationAmountY = maxRotationY - currentEulerAngleY;
                rotationAmountY = 0;
            }
            else if (currentEulerAngleY + rotationAmountY < minRotationY)
            {
                //rotationAmountY = minRotationY - currentEulerAngleY;
                rotationAmountY = 0;
            }
            transform.RotateAround(cameras[0].position, transform.up, rotationAmountY);
            //transform.Rotate(Vector3.up, rotationAmountY);
        }
        if (rotationAmountX != 0)
        {
            //Debug.Log(currentEulerAngleX + rotationAmountX);
            if ((currentEulerAngleX + rotationAmountX) > maxRotationX && (currentEulerAngleX + rotationAmountX) < 180)
            {
                rotationAmountX = 0;
                //rotationAmountX = maxRotationX - currentEulerAngleX;
            }
            else if ((currentEulerAngleX + rotationAmountX < minRotationX) && (currentEulerAngleX + rotationAmountX > 180))
            {
                rotationAmountX = 0;
                //rotationAmountX = maxRotationX - currentEulerAngleX;
            }
            //transform.Rotate(Vector3.right, rotationAmountX);
            transform.RotateAround(cameras[0].position, transform.right, rotationAmountX);
        }
#endif
#if UNITY_ANDROID
        if (Input.touchCount == 1)
        {
            transform.LookAt(cameras[0]);
            Camera.main.transform.localRotation = Quaternion.identity;
            
            if (connection)
                Camera.main.fieldOfView = 40;

            if (Physics.Raycast(cameras[0].position, transform.position - cameras[0].position, out RaycastHit hit, cameraDist, ~(1 << 6)))
            {
                Vector3 newCamPos = hit.point;
                transform.position = newCamPos;
            }
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                startPos = Input.GetTouch(0).position;
            }
            else if (Input.GetTouch(0).phase == TouchPhase.Moved)
            //if the finger has moved
            //checks the difference between the intial touch position of the finger to the new finger position
            {
                Vector2 swipeDelta = Input.GetTouch(0).position - startPos;

                float rotationAmountY = swipeDelta.x / 100f * rotationSpeed * Time.deltaTime;
                float rotationAmountX = -swipeDelta.y / 100f * rotationSpeed * Time.deltaTime;
                float currentEulerAngleY = transform.rotation.eulerAngles.y;
                float currentEulerAngleX = transform.rotation.eulerAngles.x;
                if (isPrelevel)
                {
                    if (rotationAmountX != 0)
                    { 
                        if ((currentEulerAngleX + rotationAmountX) > maxRotationX + 30f && (currentEulerAngleX + rotationAmountX) < 180)
                        {
                            rotationAmountX = 0;
                            //rotationAmountX = maxRotationX - currentEulerAngleX;
                        }
                        else if ((currentEulerAngleX + rotationAmountX) < minRotationX && (currentEulerAngleX + rotationAmountX > 180))
                        {
                            rotationAmountX = 0;
                            //rotationAmountX = maxRotationX - currentEulerAngleX;
                        }
                    }
                    transform.RotateAround(cameras[0].position, transform.up, rotationAmountY);
                    transform.RotateAround(cameras[0].position, transform.right, rotationAmountX);
                    return;
                }
                else 
                {
                    if (rotationAmountY != 0)
                    {
                        if ((currentEulerAngleY + rotationAmountY) > maxRotationY)
                        {
                            //rotationAmountY = maxRotationY - currentEulerAngleY;
                            rotationAmountY = 0;
                        }
                        else if (currentEulerAngleY + rotationAmountY < minRotationY)
                        {
                            //rotationAmountY = minRotationY - currentEulerAngleY;
                            rotationAmountY = 0;
                        }
                        transform.RotateAround(cameras[0].position, transform.up, rotationAmountY);
                        //transform.Rotate(Vector3.up, rotationAmountY);
                    }
                    if (rotationAmountX != 0)
                    {
                        //Debug.Log(currentEulerAngleX + rotationAmountX);
                        if ((currentEulerAngleX + rotationAmountX) > maxRotationX && (currentEulerAngleX + rotationAmountX) < 180)
                        {
                            rotationAmountX = 0;
                            //rotationAmountX = maxRotationX - currentEulerAngleX;
                        }
                        else if ((currentEulerAngleX + rotationAmountX < minRotationX) && (currentEulerAngleX + rotationAmountX > 180))
                        {
                            rotationAmountX = 0;
                            //rotationAmountX = maxRotationX - currentEulerAngleX;
                        }
                        //transform.Rotate(Vector3.right, rotationAmountX);
                        transform.RotateAround(cameras[0].position, transform.right, rotationAmountX);

                    }
                }
                transform.RotateAround(cameras[0].position, transform.up, rotationAmountY);
                transform.RotateAround(cameras[0].position, transform.right, rotationAmountX);
                //transform.rotation = Quaternion.Euler(0, 0, rotationAmountXtemp);
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
        if (!allowZoom)
            return;

#if UNITY_STANDALONE
        if (Input.mouseScrollDelta.y != 0)
        {
            zoomAmount += -Input.mouseScrollDelta.y * sensitivity * 50f * Time.deltaTime;
            if (zoomAmount > 100)
            {
                zoomAmount = 100;
            }
            if (zoomAmount < 20)
            {
                zoomAmount = 20;
            }
            float zoom = Mathf.Clamp(zoomAmount, 20, 100);

            Camera.main.fieldOfView = zoom;
        }
#endif
#if UNITY_ANDROID
        if (Input.touchCount == 2)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began || Input.GetTouch(1).phase == TouchPhase.Began)
            {
                initialDistance = Vector2.Distance(Input.GetTouch(0).position, Input.GetTouch(1).position);
            }
            //else if (InputSystem.Instance.isStationary(2, deltaDistance) && deltaDistance != 0)
            //{
            //    return;
            //}
            else if (Input.GetTouch(0).phase == TouchPhase.Stationary || Input.GetTouch(1).phase == TouchPhase.Stationary)
            {
                initialDistance = Vector2.Distance(Input.GetTouch(0).position, Input.GetTouch(1).position);
            }
            else if (Input.GetTouch(0).phase == TouchPhase.Moved || Input.GetTouch(1).phase == TouchPhase.Moved)
            {
                float currentDistance = Vector2.Distance(Input.GetTouch(0).position, Input.GetTouch(1).position);
                deltaDistance = currentDistance - initialDistance;

                if (InputSystem.Instance.isStationary(2, initialDistance, deltaDistance))
                {
                    return;
                }

                zoomAmount += -deltaDistance * sensitivity * Time.deltaTime;
                if (zoomAmount > 100)
                {
                    zoomAmount = 100;
                }
                if (zoomAmount < 20)
                {
                    zoomAmount = 20;
                }
                float zoom = Mathf.Clamp(zoomAmount, 20, 100);
                Camera.main.fieldOfView = zoom;
            }
            else if (Input.GetTouch(0).phase == TouchPhase.Ended || Input.GetTouch(1).phase == TouchPhase.Ended)
            {
                zoomAmount = Camera.main.fieldOfView;
            }
        }
#endif
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

    public void LookAtComponent(Transform componentToLook) 
    {
        allowRotation = false;
        zooming = true;
        transform.position = new Vector3(transform.position.x,componentToLook.position.y);
        transform.LookAt(componentToLook);
        Camera.main.fieldOfView = 10;

    }

    public void LockCamera() 
    {
        allowRotation = !allowRotation;
        if (allowRotation) 
        {
            cameraLockBtn.GetComponent<Image>().sprite = lockCam;
        }
        else 
        {
            cameraLockBtn.GetComponent<Image>().sprite = unlockCam;
        }

    }

    //-15, 100
    private void CameraScroll() 
    {
        if (!allowRotation)
            return;
        foreach (Touch touch in Input.touches)
        {
            if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
            {
                return;
            }
        }

        if (Input.touchCount > 0)
        {
            var pos = transform.position;
            pos.y = Mathf.Clamp(transform.position.y + Input.GetTouch(0).deltaPosition.y / 8, scrollFloor, scrollCeiling);
            transform.position = pos;
        }
    }
}
