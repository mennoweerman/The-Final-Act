using System;
using UnityEngine;

public class BallHitDetector : MonoBehaviour
{
    [HideInInspector] public FollowTheBall followTheBall;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            if (followTheBall != null)
            {
                followTheBall.OnBallHit(gameObject);
            }
            Destroy(collision.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            if (followTheBall != null)
            {
                followTheBall.OnBallHit(gameObject);
            }
            Destroy(other.gameObject);
        }
    }
}
