using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    [Tooltip("The speed at which the enemy will move towards player.")]
    [SerializeField]
    private float speed = 1f;

    [Tooltip("The script that handles the game logic")]
    [SerializeField]
    private RollingGame gameScript;

    [SerializeField]
    private Transform player;

    private bool willBeDestroyed = false;

    // The sound that occurs when this enemy dies
    public AudioSource dieSound;

    private void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, player.position, speed);
    }

    void OnCollisionEnter(Collision c)
    {
        // If this enemy is dying, do not consider collisions anymore.
        if (willBeDestroyed)
        {
            return;
        }

        if (c.gameObject.tag == "Player")
        {
            gameScript.GameEnded(false);
            willBeDestroyed = true;
        }
        else if (c.gameObject.tag == "Enemy")
        {
            willBeDestroyed = true;
            dieSound.PlayOneShot(dieSound.clip);

            // Begin die animation
            gameObject.GetComponent<Animator>().SetTrigger("DieTrigger");

            Invoke("DestroySelf", 1);
        }
    }

    void DestroySelf()
    {
        Destroy(this.gameObject);
    }
}
