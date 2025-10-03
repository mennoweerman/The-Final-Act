using UnityEngine;

public class CardChaos : MonoBehaviour
{
    public bool isActive;
    public GameObject cardPrefab;
    public float shootInterval = 0.5f;
    private float timer;

    public void StartPhase()
    {
        isActive = true;
        timer = 0f;
    }

    void Update()
    {
        if (!isActive) return;

        timer += Time.deltaTime;
        if (timer >= shootInterval)
        {
            ShootCardPattern();
            timer = 0f;
        }
    }

    void ShootCardPattern()
    {
        int cardCount = 12;
        for (int i = 0; i < cardCount; i++)
        {
            float angle = i * (360f / cardCount);
            Vector3 dir = Quaternion.Euler(0, 0, angle) * Vector3.right;

            GameObject card = Instantiate(cardPrefab, transform.position, Quaternion.identity);
            card.GetComponent<Rigidbody>().linearVelocity = dir * 6f;
        }
    }

    public void EndPhase()
    {
        isActive = false;
    }
}

