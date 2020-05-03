using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    [Tooltip("The script that handles the game logic")]
    [SerializeField]
    private RollingGame gameScript;

    // Tells whether the ball is grounded (and can jump)
    private bool grounded;

    [Tooltip("The height at which the ball will jump")]
    [SerializeField]
    private float jumpHeight = 1f;

    [Tooltip("The speed at which the ball will move")]
    [SerializeField]
    private float speed = 5f;

    // Where the ball spawns
    private Vector3 startingPosition;

    // The rigidbody for this player
    private Rigidbody rigidBody;

    void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    void Start()
    {
        startingPosition = transform.position;
        GetComponent<ParticleSpawner>().SpawnResetParticles(transform.position);
    }

    void Update()
    {
        // If the ball fell off the grid, respawn it
        if (Vector3.Distance(transform.position, startingPosition) > 20)
        {
            gameScript.BallOutOfBounds();
            transform.position = startingPosition;
            rigidBody.velocity = new Vector3(0, 0, 0);
            GetComponent<ParticleSpawner>().SpawnResetParticles(transform.position);
        }
        
        // If the ball is going too fast, slow it down!
        if (rigidBody.velocity.magnitude > 20f)
        {
            rigidBody.velocity = rigidBody.velocity * 0.5f;
        }
        
    }

    void OnCollisionEnter(Collision c)
    {
        if (c.gameObject.tag == "Pickup")
        {
            gameScript.PickupCollected();
            GetComponent<ParticleSpawner>().SpawnStarParticles(transform.position);
            Destroy(c.gameObject);
        }
        else if (c.gameObject.tag == "Bad Pickup")
        {
            gameScript.BadPickupCollected();
            GetComponent<ParticleSpawner>().SpawnBadParticles(transform.position);
            Destroy(c.gameObject);
        }
        else if (c.gameObject.tag == "Ground")
        {
            grounded = true;
        }
    }

    // Freeze the ball from moving
    public void FreezePlayer()
    {
        rigidBody.constraints = RigidbodyConstraints.FreezePosition;
    }

    // Unfreeze the ball
    public void UnfreezePlayer()
    {
        rigidBody.constraints = RigidbodyConstraints.None;
    }

    private void OnCollisionStay(Collision c)
    {
        if (c.gameObject.tag == "Ground")
        {
            grounded = true;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        grounded = false;
    }

    private void FixedUpdate()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        var motionVector = new Vector3(moveHorizontal, 0, moveVertical);

        rigidBody.AddForce(motionVector * speed);


        if (Input.GetKeyDown(KeyCode.Space) && grounded)
        {
            rigidBody.AddForce(new Vector3(0f, jumpHeight, 0f), ForceMode.Impulse);
        }
    }
}
