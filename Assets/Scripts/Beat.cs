using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beat : MonoBehaviour
{
    private static GameObject explosionPrefab;
    private void Awake()
    {
        if (explosionPrefab == null)
            explosionPrefab = Resources.Load<GameObject>("Explosion");
    }

    private void OnCollisionEnter(Collision collision)
    {
       
        // Paper beats Rock
        if (this.gameObject.CompareTag("Paper"))
        {
            if (collision.gameObject.CompareTag("Rock"))
            {
                collision.gameObject.SetActive(false);
                GameManager.Instance.UpdateGameState(GameState.Decide);
                Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            }

        }
        // Rock beats Scissors
        if (this.gameObject.CompareTag("Rock"))
        {
           if (collision.gameObject.CompareTag("Scissors"))
            {
                collision.gameObject.SetActive(false);
                GameManager.Instance?.UpdateGameState(GameState.Decide);
                Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            }
        }
        // Scissors beats Paper
        if (this.gameObject.CompareTag("Scissors"))
        {
            if (collision.gameObject.CompareTag("Paper"))
            {
                collision.gameObject.SetActive(false);
                GameManager.Instance?.UpdateGameState(GameState.Decide);
                Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            }
        }

    }


}
