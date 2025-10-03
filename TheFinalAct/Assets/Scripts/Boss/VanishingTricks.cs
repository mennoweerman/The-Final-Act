using UnityEngine;

public class VanishingTricks : MonoBehaviour
{
    public bool isActive;
    public GameObject bossModel;
    public GameObject[] illusions;
    public float teleportInterval = 3f;
    public float teleportMinX = -8f;
    public float teleportMaxX = 8f;
    private float timer;

    public void StartPhase()
    {
        isActive = true;
        timer = 0f;
        SpawnIllusions();
    }

    void Update()
    {
        if (!isActive) return;

        timer += Time.deltaTime;
        if (timer >= teleportInterval)
        {
            TeleportBoss();
            timer = 0f;
        }
    }

    void SpawnIllusions()
    {
        foreach (var illusion in illusions)
            illusion.SetActive(true);
    }

    void TeleportBoss()
    {
        Vector3 randomPos = new Vector3(Random.Range(teleportMinX, teleportMaxX), bossModel.transform.position.y, 0);
        bossModel.transform.position = randomPos;
    }

    public void EndPhase()
    {
        isActive = false;
        foreach (var illusion in illusions)
            illusion.SetActive(false);
    }
}
