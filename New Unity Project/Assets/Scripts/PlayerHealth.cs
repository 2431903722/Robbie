using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    int trapsLayer;
    public GameObject deathVFXPrefab;

    void Start()
    {
        trapsLayer = LayerMask.NameToLayer("Traps");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == trapsLayer)
        {
            Instantiate(deathVFXPrefab, transform.position, Quaternion.Euler(0,0,Random.Range(-45, 45)));
            gameObject.SetActive(false);
            AudioManager.PlayDeathAudio();

            //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

            GameManager.PlayerDied();
        }
    }
}
