using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Range(0, 10)]
    public float smoothingSpeed;

    public Vector3 offset;

    private Transform target;

    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, target.position + offset, Time.deltaTime * smoothingSpeed);
    }

    public void SetTarget(Transform target) => this.target = target;
}
