using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiTargetCam : MonoBehaviour
{

    [SerializeField] Transform player;
    //public List<Transform> targets;
    public float camCenter;
    Vector3 cameraCenter;
    Camera cam;


    void Start()
    {
        cam = GetComponent<Camera>();
        cameraCenter = cam.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, cam.nearClipPlane));
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(Vector2.Distance(player.position, cameraCenter));
        //Debug.Log(Mathf.Abs((player.position - cameraCenter).x));
    }
}
