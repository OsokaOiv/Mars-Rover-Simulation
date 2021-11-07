using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Waypoint : MonoSingleton<Waypoint>
{
    #region Attributes
    [SerializeField] private RectTransform WaypointTransform;
    [SerializeField] private Transform Rover;
    [SerializeField] private LayerMask GroundLayer;
    [SerializeField] private Text distanceText;
    public Queue<Vector3> Waypoints = new Queue<Vector3>();
    #endregion

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.T) && Waypoints.Count > 0)
        {
            Waypoints.Dequeue();
        }
        else if (Input.GetMouseButtonDown(0))
        {
            GetMouse3DPosition();
        }

        WaypointTransform.gameObject.SetActive(Waypoints.Count > 0);

        if (Waypoints.Count > 0)
            DisplayArrowOnWaypoint();

    }

    #region Waypoint Methods
    private void GetMouse3DPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, GroundLayer))
        {
            Waypoints.Enqueue(raycastHit.point);
        }
    }

    private void DisplayArrowOnWaypoint()
    {
        var screenPos = Camera.main.WorldToScreenPoint(Waypoints.Peek());
        WaypointTransform.position = screenPos;
        WaypointTransform.gameObject.SetActive(screenPos.z > 0);
        distanceText.text = Vector3.Distance(Rover.position, Waypoints.Peek()).ToString("0.0") + " m";
    }
    #endregion
}
