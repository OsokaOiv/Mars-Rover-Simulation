using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoSingleton<CameraMovement>
{
    #region Attributes
    [SerializeField] private Transform ResetTransform;
    [SerializeField] private float MaxVelocity = 50f;
    [SerializeField] private float NormalVelocity = 20f;
    [SerializeField] private float TurnAround = 7f;
    [SerializeField] private LayerMask LayerMask;

    private Camera Camera;
    private float Velocity;

    public Queue<Vector3> Waypoints = new Queue<Vector3>();
    #endregion

    #region Start and Update
    // Start is called before the first frame update
    void Start()
    {
        Camera = this.gameObject.GetComponent<Camera>();
        Velocity = NormalVelocity;
    }

    // Update is called once per frame
    void Update()
    {
        ChangeVelocity();
        Movement();
        Rotation();
        ResetPosition();
        if (Input.GetMouseButtonDown(0))
        {
            GetMouse3DPosition();
        }
    }
    #endregion

    #region Movement Methods
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

    private void ResetPosition()
    {
        if (Input.GetKeyDown(KeyCode.R) && ResetTransform != null)
        {
            transform.SetPositionAndRotation(ResetTransform.position, Quaternion.identity);
        }
    }
    #endregion

    #region Waypoint Methods
    private void GetMouse3DPosition()
    {
        Ray ray = Camera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, LayerMask))
        {
            Debug.Log(raycastHit.point);
            Waypoints.Enqueue(raycastHit.point);
        }
    }
    #endregion
}
