using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerStates
{
    Idle,
    Walking,
    Jump
}

public class Player : MonoBehaviour
{
    [Range(0, 20)]
    public float playerSpeed;
    [Range(0, 5)]
    public float simulationTimeInterval;
    [Range(0, 20)]
    public float jumpForce;

    public GameObject npc;

    public CameraController controller;

    private PlayerStates playerState;

    private float lastHorizontal;
    private Vector3 previousPosition;
    private bool updatedPrevPosition;
    private Rigidbody2D physics;

    public List<TrackInfo> trackedStates = new List<TrackInfo>();

    private bool simulating;
    private bool jump;
    private bool inAir;
    private bool grounded;
    private bool sentJump;

    void Start()
    {
        physics = GetComponent<Rigidbody2D>();
        controller.SetTarget(transform);
    }

    void Update()
    {
        PlayerMovement();
    }

    private void PlayerMovement()
    {
        if (simulating)
            return;

        float horizontal = Input.GetAxis("Horizontal");

        if (horizontal != 0.0f)
        {
            if(!updatedPrevPosition && trackedStates.Count <= 0)
            {
                previousPosition = transform.position;
                updatedPrevPosition = true;
            }

            Vector3 position = transform.right * Time.deltaTime * playerSpeed;
            Quaternion rotation = Quaternion.Euler(0, horizontal > 0 ? 0 : 180, 0);

            transform.position += position;
            transform.rotation = rotation;

            if (lastHorizontal != horizontal && grounded)
                UpdateState(PlayerStates.Walking, rotation);
        }
        else
        {
            if (grounded && !jump)
            {
                updatedPrevPosition = false;
                UpdateState(PlayerStates.Idle);
            }
        }


        if (Input.GetKeyDown(KeyCode.Space))
        {
            UpdateState(PlayerStates.Jump, default, jumpForce);

            jump = true;
        }



        if (Input.GetKeyDown(KeyCode.K))
        {
            SpawnNPC();
        }

        lastHorizontal = horizontal;
    }

    private void FixedUpdate()
    {
        if (jump)
        {
            physics.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);

            inAir = true;
            jump = false;
            grounded = false;
        }
    }

    private void UpdateState(PlayerStates state, Quaternion rotation = new Quaternion(), float jumpForce = 0)
    {
        if (state == playerState)
            return;

        if (playerState != PlayerStates.Idle)
        {
            Debug.Log(transform.rotation);
            TrackInfo nextTrack = new TrackInfo(playerState, transform.position, transform.rotation);

            trackedStates.Add(nextTrack);
        }

        playerState = state;
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Ground")
        {
            for (int i = 0; i < collision.contacts.Length; i++)
            {
                if (collision.contacts[i].normal.y > 0.5)
                {
                    grounded = true;
                }
            }
        }
    }

    private void SpawnNPC()
    {
        GameObject npc = Instantiate(this.npc, previousPosition, Quaternion.identity);

        if (npc)
        {
            npc.GetComponent<NPC>().Setup(previousPosition, playerSpeed, trackedStates.ToArray(), jumpForce);
        }

        trackedStates.Clear();
    }

}
