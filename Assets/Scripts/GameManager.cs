using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TMP_Text _paddleScore1Text;
    [SerializeField] private TMP_Text _paddleScore2Text;

    [SerializeField] private Transform _paddle1Transform;
    [SerializeField] private Transform _paddle2Transform;
    [SerializeField] private Transform _ballTransform;

    private int _paddleScore1;
    private int _paddleScore2;

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
        _paddleScore1++;
        _paddleScore1Text.text = _paddleScore1.ToString();
    }

    public void PaddleScored2()
    {
        _paddleScore2++;
        _paddleScore2Text.text = _paddleScore2.ToString();
    }

    public void Restart()
    {
        _paddle1Transform.position = new Vector2(_paddle1Transform.position.x, 0);
        _paddle2Transform.position = new Vector2(_paddle2Transform.position.x, 0);
        _ballTransform.position = new Vector2(0, 0);
    }
}
