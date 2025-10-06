using UnityEngine;

public enum BossPhase
{
    None,
    VanishingTricks,
    CardChaos,
    FollowTheBall
}

public class BossPhaseManager : MonoBehaviour
{
    public static BossPhaseManager Instance { get; private set; }

    [Header("Phase Scripts")]
    public VanishingTricks vanishingTricks;
    public CardChaos cardChaos;
    public FollowTheBall followTheBall;

    [Header("Current State (ReadOnly)")]
    public BossPhase currentPhase = BossPhase.None;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) StartPhase(BossPhase.VanishingTricks);
        if (Input.GetKeyDown(KeyCode.Alpha2)) StartPhase(BossPhase.CardChaos);
        if (Input.GetKeyDown(KeyCode.Alpha3)) StartPhase(BossPhase.FollowTheBall);
    }

    public void SetPhase(BossPhase newPhase)
    {
        currentPhase = newPhase;
    }

    public void StartPhase(BossPhase newPhase)
    {
        EndAllPhases();
        currentPhase = newPhase;

        switch (newPhase)
        {
            case BossPhase.VanishingTricks:
                vanishingTricks.StartPhase();
                break;
            case BossPhase.CardChaos:
                cardChaos.StartPhase();
                break;
            case BossPhase.FollowTheBall:
                followTheBall.StartPhase();
                break;
        }


    }

    public void EndAllPhases()
    {
        vanishingTricks.EndPhase();
        cardChaos.EndPhase();
        followTheBall.EndPhase();
    }
}
