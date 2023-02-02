using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class MultiTargetCam : MonoBehaviour
{

    [SerializeField] Transform player;
    public List<Transform> targets;
    public float camCenter;
    Vector3 cameraCenter;

    public Vector3 offset;

    Camera cam;

    public float smoothTime = .5f;
    public float minimumZoom = 1.76f;
    public float maximumZoom = 3f;
    public float zoomMultiplier;

    Vector3 velocity;
    void Start()
    {
        cam = GetComponent<Camera>();
        
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(Vector2.Distance(player.position, cameraCenter));
        //Debug.Log(Mathf.Abs((player.position - cameraCenter).x));
    }

    private void LateUpdate()
    {
        if (targets.Count == 0)
        {
            return;
        }

        //Move the camera
        CameraMovement();
        CameraZoom();
    }

    private void CameraZoom()
    {
        
        float newZoom = Mathf.Lerp(minimumZoom, maximumZoom, GetGreatestDistance() / zoomMultiplier);
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, newZoom, Time.deltaTime);
    }

    private float GetGreatestDistance()
    {
        var bounds = new Bounds(targets[0].position, Vector3.zero);

        for(int i = 0; i < targets.Count; i++)
        {
            bounds.Encapsulate(targets[i].position);
        }

        return bounds.size.x;
    }

    private void CameraMovement()
    {
        Vector3 centrePoint = getCentrePoint();

        Vector3 newPos = centrePoint + offset;

        transform.position = Vector3.SmoothDamp(transform.position, newPos, ref velocity, .5f);
    }

    private Vector3 getCentrePoint()
    {
        if(targets.Count == 1)
        {
            return targets[0].position;
        }

        var bounds = new Bounds(targets[0].position, Vector3.zero);

        for(int i = 0; i < targets.Count; i++)
        {
            bounds.Encapsulate(targets[i].position);
        }

        return bounds.center;
    }
}
