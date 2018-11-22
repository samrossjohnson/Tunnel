using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinPreviewPlayer : MonoBehaviour {

    public float speed;     //Speed at which the transform will travel

    Vector2[] waypoints;    //Array of Vector3 representing the corners of the ring

    Vector2 target;         //Current waypoint we are travelling to
    int index;              //Index of the target waypoint

    private void Start()
    {
        //  Initialize the waypoints array and set all four waypoints
        waypoints = new Vector2[4];
        waypoints[0] = transform.localPosition;
        waypoints[1] = new Vector2(-1.5f, 1.5f);
        waypoints[2] = new Vector2(1.5f, 1.5f);
        waypoints[3] = new Vector2(1.5f, -1.5f);

        target = waypoints[1];
        index = 1;
    }

    private void Update()
    {
        // Calculate the amount to move each frame
        float step = speed * Time.deltaTime;

        transform.localPosition = Vector3.MoveTowards(transform.localPosition, target, step);

        if ((Vector2)transform.localPosition == target)
            NextTarget();
    }

    private void NextTarget()
    {
        index++;

        // Prevent our index from going out of bounds
        if (index > waypoints.Length - 1)
            index = 0;

        target = waypoints[index];
    }
}
