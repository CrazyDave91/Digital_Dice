using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RotateNumbers : MonoBehaviour
{
    public void RotateNums()
    {
        float angle = Vector3.Dot(Vector3.right, Vector3.ProjectOnPlane(transform.right, Vector3.forward).normalized);
        angle -= 1f;
        angle /= 2f;
        angle *= 90f;
        transform.Rotate(Vector3.forward, angle);
    }
}
