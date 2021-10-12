using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarsPathfinderRover : MonoBehaviour
{
    #region Attributes
    [SerializeField] private float Velocity = .69f;
    [SerializeField] private float ScanDistance = 5f;
    [SerializeField] private float RotationSpeed = .5f;
    [SerializeField] private WheelCollider[] wheels;
    private Rigidbody rb;
    #endregion
    #region Start and FixedUpdate
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (CameraMovement.Instance.Waypoints.Count != 0)
        {
            DriveToWaypoint();
        }
    }
    #endregion

    #region Movement Methods
    private void DriveToWaypoint()
    {
        Vector3 waypoint = CameraMovement.Instance.Waypoints.Peek();
        Vector3 targetDir = new Vector3(waypoint.x, 0, waypoint.z) - new Vector3(transform.position.x, 0, transform.position.z);

        Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDir, RotationSpeed * Time.fixedDeltaTime, 0);

        foreach (WheelCollider w in wheels)
        {
            w.motorTorque = Velocity * Time.fixedDeltaTime;
            //w.steerAngle
        }
    }
    private void ArcToWaypoint()
    {
        Vector3 waypoint = CameraMovement.Instance.Waypoints.Peek();
        Vector3 targetDir = new Vector3(waypoint.x, 0, waypoint.z) - new Vector3(transform.position.x, 0, transform.position.z);

        Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDir, RotationSpeed * Time.fixedDeltaTime, 0);

        Debug.Log(newDirection);
        Debug.Log(transform.forward);

        if (newDirection == transform.forward)
            CameraMovement.Instance.Waypoints.Dequeue();

        transform.rotation = Quaternion.LookRotation(newDirection);
        //rb.velocity = (transform.forward + rb.velocity).normalized * Time.fixedDeltaTime * Velocity;
        //rb.AddRelativeForce(Vector3.forward * Time.fixedDeltaTime * Velocity);

        //transform.Translate(transform.forward * Velocity * Time.fixedDeltaTime);
        
        //leftWheel.motorTorque = Time.fixedDeltaTime * Velocity;
        //rightWheel.motorTorque = Time.fixedDeltaTime * Velocity;
    }
    #endregion
}
