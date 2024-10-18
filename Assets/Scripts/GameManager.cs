using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public float health = 100;
    public bool hasCaughtChicken;
    public bool isAttacking;
    public bool isGameOver = true;
    public bool hasWon;
    public bool isChickenScreaming;

    private GameObject _mainMenuScreen;
    private GameObject _gameOverScreen;
    private GameObject _winnerScreen;

    private void Start()
    {
        _mainMenuScreen = GameObject.Find("Main Menu");
        _gameOverScreen = GameObject.Find("Game Over");
        _winnerScreen = GameObject.Find("Winner");
        _gameOverScreen.SetActive(false);
        _winnerScreen.SetActive(false);
    }

    private void Update()
    {
        if (health <= 0)
        {
            // TODO: play dying animation
            isGameOver = true;
            _gameOverScreen.SetActive(true);
        }
    }

    public void GotAttacked()
    {
        isAttacking = true;
        health -= 0.1f;
    }

    public void CaughtChicken()
    {
        isAttacking = true;
        hasCaughtChicken = true;
        // TODO: Play chicken screaming sound 
    }

    public void ReachedJungle()
    {
        if (hasCaughtChicken && !isGameOver)
        {
            var particles = GameObject.FindGameObjectsWithTag("Confetti");
            foreach (var particle in particles) particle.GetComponent<ParticleSystem>().Play();
            _winnerScreen.SetActive(true);
            hasWon = true;
            isGameOver = true;
        }
    }

    public void StartGame()
    {
        isGameOver = false;
        _mainMenuScreen.SetActive(false);
        _gameOverScreen.SetActive(false);
        _winnerScreen.SetActive(false);
    }

    public void ToMainMenu()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public IEnumerator Delay(float seconds)
    {
        yield return new WaitForSeconds(seconds);
    }
}