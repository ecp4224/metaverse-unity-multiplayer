using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Utility;

public class FollowTarget : MonoBehaviour
{
    [SerializeField]
    private Transform target;
    [SerializeField]
    private float moveSpeed = 0.5f;
    [SerializeField]
    private float rotationSpeed = 0.4f;

    private void Start()
    {
        
    }

    private void LateUpdate()
    {
        LookDirection(target.position);
        Move();
    }

    private void LookDirection(Vector3 goal)
    {
        Vector3 lookAtGoal = new Vector3(goal.x, transform.position.y, goal.z);

        //this is direction we want this object to face
        Vector3 direction = lookAtGoal - transform.position;

        //take our current rotation and turn towards the direction we want little by little
        //at the rate of our rotation speed on a set time
        transform.rotation = Quaternion.Slerp(transform.rotation,
            Quaternion.LookRotation(direction), Time.deltaTime * rotationSpeed);
    }

    private void Move()
    {
        //push forward on z
        transform.Translate(0, 0, moveSpeed * Time.deltaTime);
    }
}
