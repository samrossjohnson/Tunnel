using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(TrailRenderer))]
public class PlayerController : MonoBehaviour {

    public float movementSpeed;                         //Speed the player moves (units/second).
    public float rotationSpeed;                         //Speed the player rotates (degrees/second).
    public float rotationLimit;                         //Angle that the player cannot rotate past.

    Vector3 targetRotation;                             //Vector3 storing angle the player is currently rotating towards

    bool gameStarted = false;                           //Has the game started?

    public LevelController levelController;             //Reference to the LevelController component.
    public CorridorSectionGenerator corridorCreator;    //Reference to the CorridorSectionGenerator component.
    public GameObject deathAnimation;                   //Reference to the death animation.

    int score;                                          //Current score.

    public AudioClip milestoneSoundEffect;              //Reference to the AudioClip tha plays every 50 score earned.


    private void Start()
    {
        // Initialize the score at 0
        score = 0;

        // Apply the equipped skin
        GetComponent<SpriteRenderer>().color = SkinManager.instance.equippedSkin.startColor;
        TrailRenderer trail = GetComponent<TrailRenderer>();
        trail.startColor = SkinManager.instance.equippedSkin.startColor;
        trail.endColor = SkinManager.instance.equippedSkin.endColor;
    }

    private void Update()
    {

        // If the game hasn't already started, check for game start.
        if (!gameStarted)
            CheckForStart();

        // If the game has started, calculate our target rotation every frame.
        if (gameStarted)
            CalculateTargetRotation();

        // Rotate the player towards the target rotation
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(targetRotation), rotationSpeed * Time.deltaTime);

        // Move the player in the direction they are facing
        transform.position += transform.up * movementSpeed * Time.deltaTime;

        // If the game has started, then we also want to move the camera
        if (gameStarted)
            Camera.main.transform.position += transform.up * movementSpeed * Time.deltaTime;

        // Increase the movement speed
        movementSpeed += Time.deltaTime * 0.04f;

        // Don't let our speed go over the maximum speed.
        if (movementSpeed > 6.5f)
            movementSpeed = 6.5f;
    }

    // Calculate the targetRotation each frame
    private void CalculateTargetRotation()
    {
        int touchCount = Input.touchCount;

        float h = 0f;

        // If a touch is detected
        if (touchCount > 0)
        {   
            // Get the Touch
            Touch touch = Input.GetTouch(0);
            if (touch.position.x > Screen.width / 2f)
                h = 1f;
            if (touch.position.x < Screen.width / 2f)
                h = -1f;
        }

        // Set the target rotation
        targetRotation = -Vector3.forward * rotationLimit * h;
    }

    // Checks if the player has passed the starting line
    private void CheckForStart()
    {
        // This is done to prevent the player being able to turn into walls and die before they are on screen
        if (transform.position.y >= -10f)
            gameStarted = true;
    }

    // Called when the player exits a Collider2D markd as a trigger
    private void OnTriggerExit2D(Collider2D collision)
    {
        // Check if the collision was tagged as a "SectionContainer"
        if (collision.tag == "SectionContainer")
            // Create a new corridor
            corridorCreator.NewCorridors(1);
    }

    // Called when the player enters a Collider2D marked as a trigger
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the collision was tagged as a "BorderTile".
        if(collision.tag == "BorderTile")
        {
            levelController.GameOver(score, movementSpeed);
            Destroy(gameObject);
            GameObject deathAnim = Instantiate(deathAnimation, transform.position, Quaternion.identity);
            ParticleSystem particle = deathAnim.GetComponent<ParticleSystem>();
            var main = particle.main;
            main.startColor = SkinManager.instance.equippedSkin.startColor; 

        }

        // Check if the collision was tagged as a "OpenTile".
        if(collision.tag == "OpenTile")
        {
            // Increase score, and update the display
            IncreaseScore();
        }
    }

    // Called when the player scores a point/
    private void IncreaseScore()
    {
        // Increase score.
        score++;

        // Update the score display.
        levelController.inGameScoreText.text = score.ToString();

        // Check if we have hit a 50 score milestone
        if (score % 50 == 0)
            // Play the milestone sound effect.
            SoundManager.instance.PlaySingleClip(milestoneSoundEffect);
    }
}
