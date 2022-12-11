using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spinner : MonoBehaviour
{
    [SerializeField]
    float zAxis = 0f;
    [SerializeField]
    float yAxis = 0f;
    [SerializeField]
    float xAxis = 0f;


    void Update()
    {
        transform.Rotate(xAxis, yAxis , zAxis, Space.Self);
    }
}
