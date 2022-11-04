using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointerHandler : MonoBehaviour
{
    public GameObject reticle;
    private LineRenderer line;

    public GameObject pointStart;

    private UserType userType;
    private Vector3[] points = new Vector3[2];

    private bool isHovered;
    // Start is called before the first frame update
    void Start()
    {
        userType = NetworkManager.User.userType;
        line = reticle.GetComponent<LineRenderer>();
    }

    private void Update()
    {
        if(userType == UserType.Lecture && isHovered)
        {
            points[0] = pointStart.transform.position;
            points[1] = reticle.transform.position;

            line.SetPositions(points);
        }
    }

    public void OnHoverEntered()
    {
        isHovered = true;
        reticle.SetActive(true);
    }

    public void OnHoverExited()
    {
        isHovered = false;
        reticle.SetActive(false);
    }
}
