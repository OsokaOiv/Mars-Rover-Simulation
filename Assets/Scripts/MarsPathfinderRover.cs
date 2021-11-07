using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarsPathfinderRover : MonoBehaviour
{
    #region Attribute
    public int timeMultiplier = 100;

    [SerializeField] float maxTurnAngle = 1;
    [SerializeField] float stepLength = .5f;
    [SerializeField] float rotationSpeed = 1f;

    private float movementSpeed = .0069f;
    #endregion

    // Wird zur Initialisierung aufgerufen
    void Start()
    {
        
    }

    // Der Kontrollzyklus wird wiederholt aufgerufen
    void Update()
    {
        if (CameraMovement.Instance.Waypoints.Count > 0) // Wenn es mehr als kein Ziel gibt
        {
            moveForward();
            rotateTowardsWaypoint(CameraMovement.Instance.Waypoints.Peek());
        }
    }

    #region Bewegung und Rotation zum Wegpunkt

    private void moveForward()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * timeMultiplier * movementSpeed);
    }

    private void rotateTowardsWaypoint(Vector3 waypoint)
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
        angle = angle < maxTurnAngle ? angle : maxTurnAngle;

        // Beurteilung ob diese Drehung nach rechts oder links erfolgen soll
        Vector2 right2D = new Vector2(transform.right.x, transform.right.z);
        Vector2 left2D = -right2D;
        if (Mathf.Acos(Vector2.Dot(vectorToWaypoint2D.normalized, left2D.normalized))*Mathf.Rad2Deg < 
            Mathf.Acos(Vector2.Dot(vectorToWaypoint2D.normalized, right2D.normalized)) * Mathf.Rad2Deg) {
            angle = -angle;
        }

        // Rotation um die y-Achse (Achse nach oben) des Rovers
        transform.Rotate(Vector3.up, angle * Time.deltaTime * timeMultiplier * rotationSpeed);
    }

    #endregion

    #region Hazard Avoidance

    #endregion
}
