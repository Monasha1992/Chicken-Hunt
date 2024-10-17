using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public float health = 100;
    public bool isAttacking;
    public bool isGameOver;
    public bool isChickenScreaming;


    private void Update()
    {
        if (health <= 0)
        {
            // TODO: play dying animation
            isGameOver = true;
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
        // TODO: Play chicken screaming sound 
    }

    public IEnumerator Delay(float seconds)
    {
        yield return new WaitForSeconds(seconds);
    }
}