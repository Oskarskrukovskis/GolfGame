using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Hole : MonoBehaviour
{
    //Components
    public Sprite redSprite;
    public Sprite blueSprite;
    public SpriteRenderer holeSpriteRenderer;
    GameObject golfBallGameObject;
    private ParticleSystem succesfulShotParticles;

    //Gameplay variables
    public HoleType holeType = HoleType.Red;

    public float totalFlashingTime = 1.5f;
    public int amountOfTimesFlashed = 15;

    bool holeEntered = false;

    //Animation variables
    public float vanishTime = 1;
    public float maxMoveTime = 0.1f;
    public float minMoveTime = 1;
    public float golfBallScale = 0;
    public float scaleTimeBuffer = 0.5f;
    public float holeDisappearAnimationTime = 0.5f;

    [SerializeField] Color blueHoleParticleColor;

    void Start()
    {
        SetupComponents();
    }

    //Gets the necessary components.
    private void SetupComponents()
    {
        golfBallGameObject = Ball.Instance.gameObject;
        succesfulShotParticles = GetComponentInChildren<ParticleSystem>();
    }

    //If ball enters the golf hole, it will fall down, disappear and spawn another one.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!holeEntered)
        {
            holeEntered = true;
            Ball.Instance.currentlyInHole = true;
            
            PlayBallFallingDownHoleAnimation();
            CheckHoleByColor();
            CheckFlashingEvent();
            StartCoroutine(BounceBack());

            AudioManager.Instance.ballAudioSource.PlayOneShot(AudioManager.Instance.ballPutSound); //Plays the falling down the hole sound.
            succesfulShotParticles.Play(); 
        }

    }

    //Plays the falling down the hole animation based on the velocity of the ball before enetering.
    private void PlayBallFallingDownHoleAnimation()
    {
        
        float timeToFallDown = Mathf.Lerp(minMoveTime, maxMoveTime, golfBallGameObject.GetComponent<Rigidbody2D>().linearVelocity.magnitude / 6);
        golfBallGameObject.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        golfBallGameObject.transform.DOMove(transform.position, timeToFallDown).SetEase(Ease.OutBounce);
        golfBallGameObject.transform.DOScale(golfBallScale, timeToFallDown - scaleTimeBuffer).SetEase(Ease.OutBounce);
    }

    //Checks whether flashing timer has come to 0 and initiates the flashing event.
    private void CheckFlashingEvent()
    {
        transform.SetParent(null); // Removes itself from the parent object so it's not counted in with the others when they are randomly selected in the flashing animation.

        if (FlashingManager.Instance.readyToFlash)
        {

            FlashingManager.Instance.FlashHoles();
            FlashingManager.Instance.ResetTimerAndStart();
        }
    }

    //Checks which hole was enetered, spawns another one of its color, plays the approprriate sound and deducts or adds time.
    private void CheckHoleByColor()
    {
        if (holeType == HoleType.Red)
        {
            TimerManager.Instance.timeLeft -= 10;
            ObjectSpawnerManager.Instance.SpawnObjectAtRandomPosition(ObjectSpawnerManager.Instance.redHolePrefab, ObjectSpawnerManager.Instance.holeparentobject);
            AudioManager.Instance.holeAudioSource.PlayOneShot(AudioManager.Instance.redHoleSound);
        }
        else
        {
            TimerManager.Instance.timeLeft += 5;
            ObjectSpawnerManager.Instance.SpawnObjectAtRandomPosition(ObjectSpawnerManager.Instance.blueHolePrefab, ObjectSpawnerManager.Instance.holeparentobject);
            AudioManager.Instance.holeAudioSource.PlayOneShot(AudioManager.Instance.blueHoleSound);
        }
    }

    //Plays the animation of the ball coming out of the hole with the hole itself disappearing and being destroyed.
    IEnumerator BounceBack()
    {
        yield return new WaitForSeconds(ObjectSpawnerManager.Instance.spawnanimationTime);
        golfBallGameObject.transform.DOScale(Ball.Instance.initialBallScale, Ball.Instance.bounceBackFromHoleTime).SetEase(Ease.OutBounce); // Scales ball back to it's deafult state
        transform.DOScale(0, holeDisappearAnimationTime); // Scales down the hole to 0
        Ball.Instance.currentlyInHole = false;
        yield return new WaitForSeconds(Ball.Instance.bounceBackFromHoleTime);
        Destroy(gameObject);
    }


    //Plays the flashing animation and sets itself to the parameter value.
    public IEnumerator FlashHole(HoleType endHoleType)
    {
        //Switches from red to blue for defined amount of times for a defined amount of time.
        for (int i = 0; i < amountOfTimesFlashed; i++)
        {
            holeSpriteRenderer.sprite = holeType == HoleType.Blue ? redSprite : blueSprite;
            if (holeType == HoleType.Blue)
            {
                holeType = HoleType.Red;
            }
            else
            {
                holeType = HoleType.Blue;
            }
            yield return new WaitForSeconds(totalFlashingTime/amountOfTimesFlashed);
        }

        holeType = endHoleType; //Sets the right type in the end.

        //Changes the visual elements to reflect it's current state.
        holeSpriteRenderer.sprite = endHoleType == HoleType.Blue ? blueSprite : redSprite;
        succesfulShotParticles.startColor = endHoleType == HoleType.Blue ? blueHoleParticleColor : Color.red;
    }

    public void InitiategameOverEvent(float gameOverTime)
    {
        StartCoroutine(GameOverCo(gameOverTime));
    }

    //Plays a disappearing animation and destroys itself.
    private IEnumerator GameOverCo(float gameOverTime)
    {
        transform.DOScale(0, gameOverTime);
        yield return new WaitForSeconds(gameOverTime);
        Destroy(gameObject);
    }
}
