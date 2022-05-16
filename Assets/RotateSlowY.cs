using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateSlowY : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up*15*Mathf.Sin(Time.deltaTime));
        Debug.Log(Mathf.Sin(Time.deltaTime));
    }
}
