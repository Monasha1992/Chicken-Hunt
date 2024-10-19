using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public float health = 100;
    public bool hasCaughtChicken;
    public bool isAttacking;
    public bool isGameOver;
    public bool gameStarted;
    public bool hasWon;

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
            gameStarted = false;
            hasWon = true;
            isGameOver = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void PlayerDied()
    {
        gameStarted = false;
        Cursor.lockState = CursorLockMode.None;
    }

    public void StartGame()
    {
        hasWon = false;
        gameStarted = true;
        isGameOver = false;
        _mainMenuScreen.SetActive(false);
        _gameOverScreen.SetActive(false);
        _winnerScreen.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
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