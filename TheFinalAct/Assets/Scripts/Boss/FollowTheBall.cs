using UnityEngine;

public class FollowTheBall : MonoBehaviour
{
    public bool isActive;
    public GameObject[] balls;
    private int realBallIndex;
    private int swapsLeft = 5;

    public void StartPhase()
    {
        isActive = true;
        realBallIndex = Random.Range(0, balls.Length);
        HighlightRealBall();
        InvokeRepeating(nameof(SwapBalls), 1f, 1f);
    }

    void SwapBalls()
    {
        if (!isActive) return;

        if (swapsLeft <= 0)
        {
            EndPhase();
            return;
        }

        int newIndex = Random.Range(0, balls.Length);
        (balls[realBallIndex].transform.position, balls[newIndex].transform.position) =
            (balls[newIndex].transform.position, balls[realBallIndex].transform.position);

        realBallIndex = newIndex;
        swapsLeft--;
    }

    void HighlightRealBall()
    {
        balls[realBallIndex].GetComponent<SpriteRenderer>().color = Color.yellow;
    }

    public void EndPhase()
    {
        isActive = false;
        CancelInvoke(nameof(SwapBalls));
    }
}
