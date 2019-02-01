using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Mouth : MonoBehaviour {

    public Vector3 mousePosition;

	// Use this for initialization
	void Start ()
    {

    }
	
	// Update is called once per frame
	void Update ()
    {
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = transform.position.z;
        Quaternion rotation = Quaternion.Euler(0, 0, MouthAngle());
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 1);
    }


    private float MouthAngle()
    {
        float angle;
        float distanceX = mousePosition.x - transform.position.x;
        float distanceY = mousePosition.y - transform.position.y;
        angle = Mathf.Rad2Deg * Mathf.Atan(distanceY / distanceX);
        if (mousePosition.x < transform.position.x)
        {
            angle -= 180;
        }
        return angle + -90;
    }


}
