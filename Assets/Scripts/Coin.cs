using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    //Components 
    [SerializeField] GameObject coinPrefab;

    //When ball collides with the coin, another one spawns, count goes up and a sound is played.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        ObjectSpawnerManager.Instance.SpawnObjectAtRandomPosition(coinPrefab, ObjectSpawnerManager.Instance.coinParentObject);

        ScoreManager.Instance.scoreCount++;
        ScoreManager.Instance.UpdateScoreUi();

        AudioManager.Instance.coinAudioSource.PlayOneShot(AudioManager.Instance.coinCollectSound);

        Destroy(gameObject);
    }

    public void InitiategameOverEvent(float gameOverTime)
    {
        StartCoroutine(GameOverCo(gameOverTime));
    }

    //Plays disappear animation and destroys itself.
    private IEnumerator GameOverCo(float gameOverTime)
    {
        transform.DOScale(0, gameOverTime);
        yield return new WaitForSeconds(gameOverTime);
        Destroy(gameObject);
    }
}
