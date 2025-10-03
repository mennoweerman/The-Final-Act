using UnityEngine;

public class BossPhaseManager : MonoBehaviour
{
    public enum BossPhase
    {
        None,
        VanishingTricks,
        CardChaos,
        FollowTheBall
    }

    [Header("Phase Scripts")]
    public VanishingTricks vanishingTricks;
    public CardChaos cardChaos;
    public FollowTheBall followTheBall;

    [Header("Current State (ReadOnly)")]
    public BossPhase currentPhase = BossPhase.None;

    void Update()
    {
        // Debug keys voor snel testen
        if (Input.GetKeyDown(KeyCode.Alpha1)) StartPhase(BossPhase.VanishingTricks);
        if (Input.GetKeyDown(KeyCode.Alpha2)) StartPhase(BossPhase.CardChaos);
        if (Input.GetKeyDown(KeyCode.Alpha3)) StartPhase(BossPhase.FollowTheBall);
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

        Debug.Log($"🔥 Phase started: {newPhase}");
    }

    void EndAllPhases()
    {
        vanishingTricks.EndPhase();
        cardChaos.EndPhase();
        followTheBall.EndPhase();
    }
}
