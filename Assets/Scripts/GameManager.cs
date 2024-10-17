using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public float health = 100;
    public bool hasCaughtChicken;
    public bool isAttacking;
    public bool isGameOver;
    public bool isChickenScreaming;

    private GameObject _gameOverScreen;
    private GameObject _winnerScreen;

    private void Start()
    {
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
        if (hasCaughtChicken) _winnerScreen.SetActive(true);
    }

    public IEnumerator Delay(float seconds)
    {
        yield return new WaitForSeconds(seconds);
    }
}