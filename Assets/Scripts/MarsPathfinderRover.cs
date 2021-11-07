using System.Collections;
using UnityEngine; // Bibliothek mit vorgefertigten Methoden

public class MarsPathfinderRover : MonoBehaviour {
    #region Attribute
    public int TimeMultiplier = 100; // Zeitmultiplikator
    [SerializeField] private LayerMask GroundLayer;
    [SerializeField] private string GroundName;
    private const float MovementSpeed = .0067f; // 0,67 cm/s
    private const float SegmentLength = .065f; // 6,5 cm
    private const float RoverDiskRadius = 1f; // 1 m
    private const float ScanningTime = 120; // 120 s
    private const float RoverHeightAboveGround = .05f;
    private const float MaxTurnAngle = 1;
    private const float RotationSpeed = .2f;
    private float DistanceToDrive = 0;
    private int Hazards = 0; // Anzahl Hindernisse im Sichtfeld
    private bool ShouldTurnToWaypoint = true; // Wird temporär auf false gesetzt, wenn ein Hindernis im Weg ist
    #endregion
    // Wird zur Initialisierung aufgerufen
    private void Start() {
        ParallelToGround(); // Rover parallel zum Boden orientieren
        StartCoroutine(ScanningCoroutine());
    }

    // Die Kontrollschleife wird wiederholt aufgerufen
    private void Update() {
        if (Waypoint.Instance.Waypoints.Count > 0) { // Wenn es mehr als 0 Wegpunkte gibt
            if (DistanceToDrive > 0 && Hazards == 0) { // Wenn er fahren kann
                if (WaypointReached(Waypoint.Instance.Waypoints.Peek())) { // Wenn der Wegpunkt erreicht wurde
                    Waypoint.Instance.Waypoints.Dequeue(); // Wegpunkt entfernen
                    return; // Nicht weiter machen
                }
                MoveForward(); // Vorwärts fahren
                if (ShouldTurnToWaypoint)
                    RotateTowardsWaypoint(Waypoint.Instance.Waypoints.Peek());
                ParallelToGround();
                if (DistanceToDrive <= 0) { // Wenn ein Segment abgefahren wurde dann scannen
                    StartCoroutine(ScanningCoroutine());
                }
            } else if (Hazards > 0) {
                RotateInPlace();
            }
        }
    }

    // Simuliert Scanzeit & beendet Scan bei keinen Hindernissen
    IEnumerator ScanningCoroutine() {
        yield return new WaitForSeconds(ScanningTime / TimeMultiplier);
        if (Hazards == 0) { NoHazardInView(); }
        else { StartCoroutine(ScanningCoroutine()); }
    } 
    #region Bewegung und Rotation zum Wegpunkt
    // Bewegt den Rover nach vorne
    private void MoveForward() {
        transform.Translate(Vector3.forward * Time.deltaTime * TimeMultiplier * MovementSpeed);
        DistanceToDrive -= Time.deltaTime * TimeMultiplier * MovementSpeed;
    }

    // Rotiert den Rover parallel zum Boden
    private void ParallelToGround() {
        Ray ray = new Ray(transform.position, Vector3.down); // Checkt den Boden direkt unter dem Rover mit einem Ray
        if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, GroundLayer)) { 
            transform.position = new Vector3(transform.position.x, raycastHit.point.y + RoverHeightAboveGround, transform.position.z);
            transform.rotation = Quaternion.FromToRotation(transform.up, raycastHit.normal) * transform.rotation; // Parallel zum Boden
        }
    }

    // Rotiert den Rover zum Wegpunkt
    private void RotateTowardsWaypoint(Vector3 waypoint) {
        // Berechnung vom VeKtor von der Rover Position zum Wegpunkt
        Vector3 vectorToWaypoint = waypoint - transform.position;

        // Konvertieren von 3D zu 2D Koordinate der Vektoren
        Vector2 vectorToWaypoint2D = new Vector2(vectorToWaypoint.x, vectorToWaypoint.z);
        Vector2 forward2D = new Vector2 (transform.forward.x, transform.forward.z);

        // Berechnung des Winkels
        float radians = Mathf.Acos(Vector2.Dot(vectorToWaypoint2D.normalized, forward2D.normalized));
        float angle = radians * Mathf.Rad2Deg;

        // Maximale Drehung von MaxTurnAngle
        angle = angle < MaxTurnAngle ? angle : MaxTurnAngle;

        // Beurteilung ob Drehung nach rechts oder links erfolgen soll
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
        // Wenn die x-Koordinate vom Wegpunkt nicht in der Scheibe ist
        if (waypoint.x <= transform.position.x - RoverDiskRadius || waypoint.x >= transform.position.x + RoverDiskRadius)
        { return false; }
        // Wenn die z-Koordinate vom Wegpunkt nicht in der Scheibe ist
        else if (waypoint.z <= transform.position.z - RoverDiskRadius || waypoint.z >= transform.position.z + RoverDiskRadius) 
        { return false; }
        else { return true; }
    } 
    #endregion 
    #region Hazard Avoidance 
    // Wird aufgerufen, wenn ein Hindernis in das Sichtfeld vom Rover kommt
    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.name == GroundName)
            return;
        ShouldTurnToWaypoint = false;
        Hazards++;
    }

    // Wird aufgerufen, wenn ein Hindernis das Sichtfeld verlässt
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == GroundName)
            return;
        Hazards--;
    }

    // Stellt die Distanz ein
    private void NoHazardInView() {
        DistanceToDrive = SegmentLength;
        ShouldTurnToWaypoint = true;
    }

    // Auf der Stelle nach rechts drehen
    private void RotateInPlace() { transform.Rotate(Vector3.up, MaxTurnAngle * Time.deltaTime * TimeMultiplier * RotationSpeed); } 
    #endregion
}
