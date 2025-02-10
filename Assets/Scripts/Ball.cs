using UnityEngine;
using System.Collections;
using DG.Tweening;

public class Ball : MonoBehaviour
{
    //Components
    public LineRenderer trajectoryLine;
    public GameObject selectCircle;

    [HideInInspector] public Rigidbody2D rb;

    // Gameplay Variables
    public float speedMultiplier = 10f;
    public float minVelocity = 0.1f;
    public float minDragDistance = 0.1f;
    public float maxDragDistance = 1f;

    private Vector2 startPosition;
    private Vector2 endPosition;

    [HideInInspector] public bool gameOver = false;
    [HideInInspector] public bool currentlyInHole = false;

    private bool isAiming = false;
    private bool startedAiming = false;
    private bool controlEnabled = false;
    private bool ballReadytoShoot = false;

    //Animation variables
    public float selectBallScale = 0.14f;
    public float selectAnimationTime = 0.2f;
    public float readyCircleScale = 0.9f;
    public float readyCircleAnimationTime = 0.5f;
    public float bounceBackFromHoleTime = 1;

    [HideInInspector] public float initialBallScale = 0.13f;

    private static Ball instance;
    public static Ball Instance
    {
        get
        {
            if (instance == null)
                instance = FindFirstObjectByType(typeof(Ball)) as Ball;

            return instance;
        }
        set
        {
            instance = value;
        }
    }

    void Start()
    {
        SetupComponents();
        initialBallScale = transform.localScale.x;
    }

    void Update()
    {
        if (!gameOver)
        {
            if (controlEnabled)
            {
                HandleInput();
            }
            else
            {
                CheckIfBallHasStopped();
            }
        }

    }

    // Checks whether ball has reached minimum velocity, then stops it and sets it up for the next shot.
    private void CheckIfBallHasStopped()
    {
        if (!currentlyInHole && rb.linearVelocity.magnitude <= minVelocity && !ballReadytoShoot)
        {
            rb.linearVelocity = Vector2.zero;
            if (!IsInvoking(nameof(ShowBallIsReady)))
            {
                ShowBallIsReady();
            }
        }
    }

    //Gets the necessary components.
    private void SetupComponents()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    
    private void HandleInput()
    {
        //MOBILE INPUTS
        /*if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector2 touchPosition = Camera.main.ScreenToWorldPoint(touch.position);

            if (touch.phase == TouchPhase.Moved) // Press and move on screen to start aiming from the position on the screen that was pressed.
            {
                Aim(touchPosition);
            }
            else if (touch.phase == TouchPhase.Ended) // Release to shoot
            {
                ShootBall();
            }
        }*/

        //PC INPUTS
        if(Input.GetMouseButton(0)) // Click to start aiming from the position on the screen that was clicked.
        {
            Aim(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        }
        
        if (Input.GetMouseButtonUp(0)) // Release to shoot
        {
            ShootBall();
        }

    }

    //Aims the ball based on the position that the screen was clicked.
    private void Aim(Vector2 position)
    {
        if (!startedAiming)
        {
            SetupAim(position);
        }
        else
        {
            ControlAim(position);
        }
    }

    //Sets up the start position of the aim and the trajectory line object.
    private void SetupAim(Vector2 position)
    {
        startPosition = position;
        trajectoryLine.positionCount = 2;
        trajectoryLine.enabled = true;
        transform.DOScale(selectBallScale, selectAnimationTime); //Scales up the ball, for the visual feedback of it being selected.
        startedAiming = true;
    }

    //Controls the aim based on the players input.
    private void ControlAim(Vector2 position)
    {
        endPosition = position;
        Vector2 direction = (startPosition - endPosition).normalized;
        DrawTrajectory(direction);
    }

    //Draws the line's trajectory.
    private void DrawTrajectory(Vector2 direction)
    {
        float trajectoryDistance = Mathf.Clamp(Vector2.Distance(startPosition, endPosition), 0, maxDragDistance);
        trajectoryLine.SetPosition(0, transform.position);
        Vector3 calculatedEndPosition = (Vector2)transform.position + direction * trajectoryDistance;
        trajectoryLine.SetPosition(1, calculatedEndPosition);
    }

    //Shoots the ball.
    private void ShootBall()
    {
        if (!startedAiming) return; // If aiming wasn't started, exit early

        ResetAimingAndControlStates();

        transform.DOScale(initialBallScale, selectAnimationTime); // Scales the ball back to it's default size

        float trajectoryDistance = Mathf.Clamp(Vector2.Distance(startPosition, endPosition), 0, maxDragDistance); // Calculate trajectory within limits.

        //If drag is too small, cancel the shot.
        if (trajectoryDistance < minDragDistance)
        {
            isAiming = false;
            trajectoryLine.enabled = false;
            return;
        }

        //Calculate direction and move the ball.
        Vector2 direction = (startPosition - endPosition).normalized;
        rb.AddForce(direction * speedMultiplier * trajectoryDistance, ForceMode2D.Impulse);

        //Reset the trajectory line.
        trajectoryLine.SetPosition(0, Vector3.zero);
        trajectoryLine.SetPosition(1, Vector3.zero);

        AudioManager.Instance.ballAudioSource.PlayOneShot(AudioManager.Instance.ballMoveSound);
    }

    // Reset these states when shooting the ball.
    private void ResetAimingAndControlStates()
    {
        startedAiming = false;
        controlEnabled = false;
        ballReadytoShoot = false;
        isAiming = false;
        trajectoryLine.enabled = false;
    }

    public void ShowBallIsReady()
    {
        StartCoroutine(SetupBallForNextShot());
    }

    //Sets up ball for the next shot and switches all the necessary states.
    public IEnumerator SetupBallForNextShot()
    {
        if (ballReadytoShoot) yield break;

        ballReadytoShoot = true;
        gameOver = false;

        //Plays the animation of the select circle
        selectCircle.transform.DOScale(readyCircleScale, readyCircleAnimationTime);
        yield return new WaitForSeconds(readyCircleAnimationTime);
        selectCircle.transform.DOScale(0, readyCircleAnimationTime);

        AudioManager.Instance.generalAudioSource.PlayOneShot(AudioManager.Instance.ballReadySound);

        controlEnabled = true;
    }

    //Plays a sound when the ball hits a wall
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            AudioManager.Instance.ballAudioSource.PlayOneShot(AudioManager.Instance.ballMoveSound);
        }
    }

    //Resets the ball and it's trajectory line after a game over
    public void ResetBallAfterGameOver()
    {
        gameOver = true;
        trajectoryLine.enabled = false;
        rb.linearVelocity = Vector2.zero;

        //If ball is in the hole during game over, then it will be scaled back to it's default size.
        if (currentlyInHole)
        {
            transform.DOScale(initialBallScale, bounceBackFromHoleTime).SetEase(Ease.OutBounce);
        }
    }
}