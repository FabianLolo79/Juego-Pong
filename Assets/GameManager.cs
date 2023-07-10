using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TMP_Text paddleScore1Text;
    [SerializeField] private TMP_Text paddleScore2Text;

    [SerializeField] private Transform paddle1Transform;
    [SerializeField] private Transform paddle2Transform;
    [SerializeField] private Transform ballTransform;

    private int paddleScore1;
    private int paddleScore2;

    private static GameManager instance;

    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameManager>();
            }
            return instance;
        }
    }

    public void PaddleScored1()
    {
        paddleScore1++;
        paddleScore1Text.text = paddleScore1.ToString();
    }

    public void PaddleScored2()
    {
        paddleScore2++;
        paddleScore2Text.text = paddleScore2.ToString();
    }

    public void Restart()
    {
        paddle1Transform.position = new Vector2(paddle1Transform.position.x, 0);
        paddle2Transform.position = new Vector2(paddle2Transform.position.x, 0);
        ballTransform.position = new Vector2(0, 0);
    }
}
