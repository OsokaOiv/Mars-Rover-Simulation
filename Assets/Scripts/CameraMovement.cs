using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private Transform ResetPosition;

    [SerializeField] private float MaxVelocity = 50f;
    [SerializeField] private float NormalVelocity = 20f;
    [SerializeField] private float TurnAround = 7f;

    private float Velocity;
    // Start is called before the first frame update
    void Start()
    {
        Velocity = NormalVelocity;
    }

    // Update is called once per frame
    void Update()
    {
        ChangeVelocity();
        Movement();
        Rotation();
        
        if (Input.GetKeyDown(KeyCode.R) && ResetPosition != null)
        {
            transform.SetPositionAndRotation(ResetPosition.position, Quaternion.identity);
        }
    }

    private void ChangeVelocity()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            Velocity = MaxVelocity;
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            Velocity = NormalVelocity;
        }
    }

    private void Movement()
    {
        if (Input.GetKey(KeyCode.W))
        {
            this.gameObject.transform.Translate(Vector3.forward * Velocity * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            this.gameObject.transform.Translate(Vector3.back * Velocity * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.A))
        {
            this.gameObject.transform.Translate(Vector3.left * Velocity * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            this.gameObject.transform.Translate(Vector3.right * Velocity * Time.deltaTime);
        }
    }

    private void Rotation()
    {
        if (Input.GetMouseButton(1))
        {
            transform.eulerAngles += TurnAround * new Vector3(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), 0);

        }
    }
}
