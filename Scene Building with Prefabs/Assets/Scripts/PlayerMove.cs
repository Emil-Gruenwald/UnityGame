using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
     float horizontalInput;
float verticalInput;
 Vector3 moveDirection;
 public Transform orientation;
 Rigidbody rb;
    
void Start() 
{
    rb = GetComponent<Rigidbody>();
    rb.freezeRotation = true;
}
    // Update is called once per frame
    void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        transform.Translate(moveDirection * Time.deltaTime);
    }
}
