using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraScript : MonoBehaviour {

    Camera camera;
    float height = 12;
    float speed = 0.05f;

    // Use this for initialization
    void Start () {
        camera = GetComponent<Camera>();
	}

    //move the camera down
    public void moveCamera()
    {
        camera.transform.position = camera.transform.position + Vector3.down;
    }

    // Update is called once per frame
    void Update () {
        float currentheight = camera.transform.position.y;

        //slowly move the camera until it is at the correct height
        if(currentheight < height)
        {
            camera.transform.position = camera.transform.position + (Vector3.up * speed);
        }
	}
}
