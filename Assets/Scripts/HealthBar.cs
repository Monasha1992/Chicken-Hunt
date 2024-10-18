using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    private GameManager _gameManager;
    private Slider _slider;

    // Start is called before the first frame update
    void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
        _slider = GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        _slider.value = _gameManager.health;
    }
}