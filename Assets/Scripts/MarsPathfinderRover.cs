using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarsPathfinderRover : MonoBehaviour
{
    #region Attribute
    public int TimeMultiplier = 100;

    [SerializeField] private LayerMask GroundLayer;

    private float MovementSpeed = .0069f;
    private float MaxTurnAngle = 1;
    private float RotationSpeed = .2f;
    private float RoverDiskRadius = 2.5f;
    private float RoverHeightAboveGround = .05f;
    private float RoverWedge = 5;
    private bool rotateToWaypoint = true; // Wird für kurze Zeit auf false gesetz, wenn ein Hindernis im Weg ist
    #endregion

    // Wird zur Initialisierung aufgerufen
    void Start()
    {
        ParallelToGround();
    }

    // Der Kontrollzyklus wird wiederholt aufgerufen
    void Update()
    {
        if (CameraMovement.Instance.Waypoints.Count > 0) // Wenn es mehr als 0 Ziele gibt
        {
            // Wenn ein Hindernis im Sichtfeld ist
            if (HazardInView())
            {

            }
            else // sonst
            {
                // Wenn der Wegpunkt erreicht wurde
                if (WaypointReached(CameraMovement.Instance.Waypoints.Peek()))
                {
                    CameraMovement.Instance.Waypoints.Dequeue(); // Wegpunkt entfernen
                    return; // Nicht weiter machen
                }

                MoveForward();
                if (rotateToWaypoint)
                    RotateTowardsWaypoint(CameraMovement.Instance.Waypoints.Peek());
                ParallelToGround();
            }
        }
    }

    #region Bewegung und Rotation zum Wegpunkt

    // Bewegt den Rover nach vorne
    private void MoveForward()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * TimeMultiplier * MovementSpeed);
    }

    // Rotiert den Rover parallel zum Boden
    private void ParallelToGround()
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, GroundLayer))
        {
            // Berechnung der y Koordinate
            transform.position = new Vector3(transform.position.x, raycastHit.point.y + RoverHeightAboveGround, transform.position.z);
            // Rotation parallel zum Boden
            transform.rotation = Quaternion.FromToRotation(transform.up, raycastHit.normal) * transform.rotation;
        }
    }

    // Rotiert den Rover zum Wegpunkt
    private void RotateTowardsWaypoint(Vector3 waypoint)
    {
        // Berechnung vom Vector von der Rover Position zum Wegpunkt
        Vector3 vectorToWaypoint = waypoint - transform.position;

        // Konvertieren von 3D zu 2D Koordinate der Vectoren
        Vector2 vectorToWaypoint2D = new Vector2(vectorToWaypoint.x, vectorToWaypoint.z);
        Vector2 forward2D = new Vector2 (transform.forward.x, transform.forward.z);

        // Berechnung des Winkels
        float radians = Mathf.Acos(Vector2.Dot(vectorToWaypoint2D.normalized, forward2D.normalized));
        float angle = radians * Mathf.Rad2Deg;

        // Maximale Drehung von maxTurnAngle
        angle = angle < MaxTurnAngle ? angle : MaxTurnAngle;

        // Beurteilung ob diese Drehung nach rechts oder links erfolgen soll
        Vector2 right2D = new Vector2(transform.right.x, transform.right.z);
        Vector2 left2D = -right2D;
        if (Mathf.Acos(Vector2.Dot(vectorToWaypoint2D.normalized, left2D.normalized))*Mathf.Rad2Deg < 
            Mathf.Acos(Vector2.Dot(vectorToWaypoint2D.normalized, right2D.normalized)) * Mathf.Rad2Deg) {
            angle = -angle;
        }

        // Rotation um die y-Achse (Achse nach oben) des Rovers
        transform.Rotate(Vector3.up, angle * Time.deltaTime * TimeMultiplier * RotationSpeed);
    }

    // Gibt true zurück wenn der Wegpunkt erreicht wurde
    private bool WaypointReached(Vector3 waypoint)
    {
        // Wenn die x-Koordinate vom Wegpunkt nicht in der 5m Scheibe ist
        if (waypoint.x <= transform.position.x - RoverDiskRadius || waypoint.x >= transform.position.x + RoverDiskRadius)
        {
            return false;
        }
        // Wenn die z-Koordinate vom Wegpunkt nicht in der 5m Scheibe ist
        else if (waypoint.z <= transform.position.z - RoverDiskRadius || waypoint.z >= transform.position.z + RoverDiskRadius)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    #endregion

    #region Hazard Avoidance

    // Gibt true zurück wenn ein Hindernis im 2,5 Meter Radius ist
    private bool HazardInView()
    {
        return false;
    }

    #endregion
}
