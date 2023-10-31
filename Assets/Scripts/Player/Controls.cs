using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Controls : MonoBehaviour
{
    public static GameObject player;
    public static Camera playerCamera;
    public static bool rotateCamera = true;
    public static bool rotatePlayer = false;

    Vector3 direction;

    void Update()
    {
        direction = playerCamera.transform.position.normalized;
        
        CameraSpin();
        PlayerSpin();
        Scroll();
    }

    void CameraSpin()
    {
        if (!rotateCamera) return;

        playerCamera.transform.RotateAround(Vector3.zero, Vector3.up, 5 * Time.deltaTime);
    }
    void PlayerSpin()
    {
        if (!rotatePlayer) return;

        player.transform.position = direction * 2880;
    }
    void Scroll()
    {
        if (Input.GetAxis("Mouse ScrollWheel") == 0) return;

        playerCamera.transform.position += direction * Vector3.Distance(playerCamera.transform.position, Vector3.zero) * -Input.GetAxis("Mouse ScrollWheel");
    }
}

//// Camera
//public float sens = 100f;
//public float flySpeed = 10f;
//public float rotationSpeed = 20f;
//public Transform cameraHolder;
//public Transform body;

//void Start()
//{
//Cursor.lockState = CursorLockMode.Locked;
//rb = gameObject.GetComponent<Rigidbody>();
//cameraDistance = cameraHolder.position - body.position;
////cameraRotation = camera.transform.rotation.eulerAngles;
//}

//// Movement
//Rigidbody rb;
//Vector3 cameraDistance;

//void MouseLook() {
//    float mouseX = Input.GetAxis("Mouse X") * sens * Time.deltaTime;
//    float mouseY = Input.GetAxis("Mouse Y") * sens * Time.deltaTime;
//    Vector3 rotation = Vector3.up * mouseX + Vector3.right * mouseY;
//    gameObject.transform.Rotate(rotation);
//}

//void Movement() {
//    float horizontal = Input.GetAxis("Horizontal");
//    float vertical = -Input.GetAxis("Vertical");
//
//    // W S
//    rb.velocity = body.transform.forward * flySpeed * vertical;
//    // A D
//    gameObject.transform.Rotate(0f, 0f, rotationSpeed * horizontal * Time.deltaTime);
//        
//    //cameraHolder.transform.position = body.transform.position + cameraDistance;
//    //gameObject.transform.rotation = body.transform.localRotation;
//    // A D
//    //Quaternion targetRotation = Quaternion.Euler(0f, 0f, rotationSpeed * horizontal * Time.deltaTime);
//    //Quaternion targetRotation = Quaternion.Euler(0f, 0f, rotationSpeed * horizontal * Time.deltaTime);
//        
//    //gameObject.transform.rotation = cameraHolder.rotation;
//    //gameObject.transform.position = new Vector3(
//    //    cameraHolder.forward.x * cameraDistance.x,
//    //    cameraHolder.forward.y * cameraDistance.y,
//    //    cameraHolder.forward.z * cameraDistance.z);
//    //gameObject.transform.localEulerAngles += new Vector3(0, 0, gameObject.transform.forward.z * horizontal * Time.deltaTime);
//}