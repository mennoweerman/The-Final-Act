using System.Collections;
using UnityEngine;

public class FollowTheBall : MonoBehaviour
{
    [Header("Ball Settings")]
    public GameObject[] balls;
    
    [Header("Ball Materials")]
    [SerializeField] public Material defaultMaterial;
    [SerializeField] public Material highlightMaterial;
    
    [Header("Timing Settings")]
    public float revealTime = 2f;
    public int swapsCount = 5;
    public float swapSpeed = 1f;
    public float stunDuration = 3f;

    [Header("Refs")]
    public BossHealth bossHealth;
    public Transform bossGameObject;
    public Vector3 followTheBallLocation = new Vector3(-0.003327415f, 0f, -16.20056f);
    
    [Header("Player Reset")]
    public Vector3 playerResetPosition = new Vector3(0, 1, 0);

    private int realBallIndex;
    private bool phaseActive = false;
    private bool canShoot = false;

    void Start()
    {
        foreach (var ball in balls)
        {
            ball.SetActive(false);
            
            BallHitDetector hitDetector = ball.GetComponent<BallHitDetector>();
            if (hitDetector == null)
            {
                hitDetector = ball.AddComponent<BallHitDetector>();
            }
            hitDetector.followTheBall = this;
        }
    }

    public void StartPhase()
    {
        phaseActive = true;
        if (BossPhaseManager.Instance != null)
        {
            BossPhaseManager.Instance.SetPhase(BossPhase.FollowTheBall);
        }

        // Verplaats boss naar FollowTheBall locatie
        if (bossGameObject != null)
        {
            bossGameObject.position = followTheBallLocation;
        }
        
        // Reset speler positie en rotatie
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            CharacterController playerController = player.GetComponent<CharacterController>();
            if (playerController != null)
            {
                playerController.enabled = false;
                player.transform.position = playerResetPosition;
                player.transform.rotation = Quaternion.identity;
                playerController.enabled = true;
            }
            else
            {
                player.transform.position = playerResetPosition;
                player.transform.rotation = Quaternion.identity;
            }
            
            // Reset camera rotatie ook
            PlayerController playerControlScript = player.GetComponent<PlayerController>();
            if (playerControlScript != null && playerControlScript.cameraTransform != null)
            {
                playerControlScript.cameraTransform.localRotation = Quaternion.identity;
            }
        }
        
        StartCoroutine(PhaseRoutine());
    }

    private IEnumerator PhaseRoutine()
    {
        phaseActive = true;

        foreach (var ball in balls)
        {
            ball.SetActive(true);
            ball.GetComponent<MeshRenderer>().material = defaultMaterial;
        }

        realBallIndex = Random.Range(0, balls.Length);
        balls[realBallIndex].GetComponent<MeshRenderer>().material = highlightMaterial;

        yield return new WaitForSeconds(revealTime);

        balls[realBallIndex].GetComponent<MeshRenderer>().material = defaultMaterial;

        yield return StartCoroutine(SwapRoutine());

        canShoot = true;
    }

    private IEnumerator SwapRoutine()
    {
        for (int i = 0; i < swapsCount; i++)
        {
            int newIndex = Random.Range(0, balls.Length);
            if (newIndex == realBallIndex) continue;

            Vector3 posA = balls[realBallIndex].transform.position;
            Vector3 posB = balls[newIndex].transform.position;

            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime * swapSpeed;
                balls[realBallIndex].transform.position = Vector3.Lerp(posA, posB, t);
                balls[newIndex].transform.position = Vector3.Lerp(posB, posA, t);
                yield return null;
            }

            realBallIndex = newIndex;
        }
    }

    public void OnBallHit(GameObject hitBall)
    {
        if (!phaseActive || !canShoot) return;

        int hitIndex = System.Array.IndexOf(balls, hitBall);

        if (hitIndex == realBallIndex)
        {
            StartCoroutine(HandleStun());
        }
        else
        {
            RestartPhase();
        }
    }

    private IEnumerator HandleStun()
    {
        canShoot = false;
        if (bossHealth != null)
        {
            bossHealth.SetStunned(true, stunDuration);
        }

        yield return new WaitForSeconds(stunDuration);

        EndPhase();
    }

    private void RestartPhase()
    {
        StopAllCoroutines();
        canShoot = false;
        StartCoroutine(PhaseRoutine());
    }

    public void EndPhase()
    {
        phaseActive = false;

        foreach (var ball in balls)
            ball.SetActive(false);
    }
}
