using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollisionHandler : MonoBehaviour
{
    public GameObject uiManager; // Reference to the UIManager
    public string blackHoleTag = "BlackHole"; // Tag for the Black Hole object
    public string monsterTag = "Monster"; // Tag for the Monster object
    public string fallTag = "FallCollider"; // Tag for the FallCollider object
    public float jumpForce = 30f; // The jump force when Doodle destroys a monster (can be set via Inspector)
    public float suckSpeed = 0.5f; // The speed at which the Doodle is sucked into the black hole
    private bool isBeingSucked = false; // Flag to check if the Doodle is being sucked into the black hole

    void Update()
    {
        if (isBeingSucked)
        {
            Debug.Log("Doodle's scale: " + transform.localScale);
            MoveTowardsBlackHole();
        }
    }

    // Called when another collider enters the trigger collider attached to this object.
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(blackHoleTag))
        {
            PlayerControl playerControl = other.GetComponent<PlayerControl>();
            playerControl.SetDead(true, this);
            HandleBlackHoleEntry(other);
        }
        else if (other.CompareTag(monsterTag))
        {
            HandleMonsterCollision(other);
        }else if (other.CompareTag(fallTag)){
            HandleLoseCondition();
        }
    }

    // Handles entry into a black hole.
    void HandleBlackHoleEntry(Collider2D doodle)
    {
        Debug.Log("Doodle entered the black hole!");

        // Disable Doodle's gravity
        if (doodle.TryGetComponent<Rigidbody2D>(out var doodleRb))
        {
            doodleRb.gravityScale = 0; // Disable gravity
            doodleRb.velocity = Vector2.zero; // Stop movement
        }

        Animator doodleAnimator = doodle.GetComponent<Animator>();
        doodleAnimator.SetBool("isBlackHoleDeath", true); // Play black hole animation
        Debug.Log("Sucking started!");
        isBeingSucked = true; // Start sucking movement
    }

    // Moves Doodle towards the black hole center.
    void MoveTowardsBlackHole()
    {
        Transform doodleTransform = transform;
        Transform blackHoleTransform = GameObject.FindGameObjectWithTag(blackHoleTag).transform;

        if (doodleTransform != null && blackHoleTransform != null)
        {
            // Lerp Doodle towards the black hole
            doodleTransform.position = Vector2.Lerp(doodleTransform.position, blackHoleTransform.position, suckSpeed * Time.deltaTime);

            // When close enough to the black hole, stop the movement
            float distance = Vector2.Distance(doodleTransform.position, blackHoleTransform.position);
            if (distance < 0.1f)
            {
                Debug.Log("Doodle reached the black hole!");
                isBeingSucked = false; // Stop sucking
                HandleLoseCondition();
            }
        }
    }

    // Handles collision with a monster.
    void HandleMonsterCollision(Collider2D monster)
    {
        if (IsHitFromAbove(monster))
        {
            Destroy(monster.gameObject); // Destroy the monster
            ApplyJumpForce();
        }
        else
        {
            // Activate StarEffects under Doodle
            Transform starEffectsTransform = transform.Find("StarEffects");
            if (starEffectsTransform != null)
            {
                GameObject starEffects = starEffectsTransform.gameObject;
                starEffects.SetActive(true);  // Activate StarEffects effect
            }
            else
            {
                Debug.LogError("StarEffects not found under Doodle.");
            }

            PlayerControl playerControl = GetComponent<PlayerControl>();
            playerControl.SetDead(true, this);
            HandleLoseCondition();
        }
    }

    // Checks if Doodle is hitting the monster from above.
    bool IsHitFromAbove(Collider2D monster)
    {
        float doodleBottomY = transform.position.y - GetComponent<Collider2D>().bounds.extents.y;
        float monsterTopY = monster.transform.position.y + monster.bounds.extents.y;
        return doodleBottomY >= monsterTopY;
    }

    // Applies jump force to Doodle after killing a monster.
    void ApplyJumpForce()
    {
        Rigidbody2D doodleRb = GetComponent<Rigidbody2D>();
        if (doodleRb != null)
        {
            doodleRb.velocity = new Vector2(doodleRb.velocity.x, 0); // Reset vertical velocity
            doodleRb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse); // Apply the jump force
        }
    }

    // Handles the lose condition and triggers the end page.
    void HandleLoseCondition()
    {
        Debug.Log("Doodle lost!");
        GameManager gameManager = FindObjectOfType<GameManager>();
        gameManager.SetIsDead(true);
        GetComponent<Collider2D>().enabled = false; // Disable further collisions

        ScoreManager scoreManager = FindObjectOfType<ScoreManager>();
        scoreManager.OnPlayerDeath();
        GetComponent<Rigidbody2D>().velocity = Vector2.zero; // Stop player movement

        UIManager uiManager = FindObjectOfType<UIManager>();
        uiManager.TriggerEndPage(gameObject.tag);
    }

}
