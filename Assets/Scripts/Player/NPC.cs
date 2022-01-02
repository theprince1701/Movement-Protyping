using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public bool cycle;
    private Vector3 previousPosition;
    private float playerSpeed;
    private float jumpForce;

    private Rigidbody2D physics;

    public List<TrackInfo> trackedStates = new List<TrackInfo>();


    public void Setup(Vector3 previousPosition, float playerSpeed, TrackInfo[] states, float jumpForce)
    {
        foreach (TrackInfo s in states)
            trackedStates.Add(s);

        physics = GetComponent<Rigidbody2D>();

        this.previousPosition = previousPosition;
        this.playerSpeed = playerSpeed;
        this.jumpForce = jumpForce;

        transform.position = previousPosition;
        StartCoroutine(LoopThroughStates(0));
    }

    private IEnumerator LoopThroughStates(int i)
    {
        float timeToComplete = (transform.position - trackedStates[i].position).magnitude / playerSpeed;

        float t = 0;

        if (trackedStates[i].state == PlayerStates.Jump)
        {
            physics.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
        }

        while (t < timeToComplete)
        {
            Vector3 position = transform.right * Time.deltaTime * playerSpeed;
            Quaternion rotation = trackedStates[i].rot;

            transform.rotation = rotation;
            transform.position += position;

            t += Time.deltaTime;

            yield return null;
        }

        int nextIteration = i + 1 >= trackedStates.Count ? 0 : i + 1;
        if (nextIteration != 0) 
            StartCoroutine(LoopThroughStates(nextIteration));
        else
        {
            if (cycle)
            {
                transform.position = previousPosition;

                StartCoroutine(LoopThroughStates(0));
            }
        }
    }
}
