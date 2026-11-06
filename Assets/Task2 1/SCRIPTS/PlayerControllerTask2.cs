using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerControllerTask2 : MonoBehaviour
{
    [SerializeField] int Score = 0;
    [SerializeField] float Timer = 60f;
    [SerializeField] bool LevelComplete = false;
    private int Ballcount = 0;

    //---------------------------------------------

    [SerializeField] TextMeshProUGUI ScoreTextBox;
    [SerializeField] TextMeshProUGUI TimerTextBox;
    [SerializeField] TextMeshProUGUI CompletionTextBox;
    [SerializeField] AudioSource PlayerSound;
    [SerializeField] AudioClip coinClip;
    [SerializeField] AudioClip gameOverClip;
    [SerializeField] Rigidbody physicsBody;
    //-----------------------------------------------------------


    [SerializeField] float moveSpeed = 20;




    void OnMouseDown()
    {
        PlayerSound.PlayOneShot(coinClip);



    }

    void ChangeName()
    {
        name = "Player Ball"; //change the name of the object that has this script.
    }

    private void OnTriggerEnter(Collider other)
    {
        if (PlayerSound != null)
        {
            PlayerSound.PlayOneShot(coinClip);
        }
        Score = Score + 1;
        Destroy(other.gameObject);
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ChangeName();
        EditorApplication.Beep();
        //--------------------- rigidbody components in code than serilization
        physicsBody.GetComponent<Rigidbody>();
        bool gravityActive = physicsBody.useGravity;
        physicsBody.linearDamping = 4;
        physicsBody.Sleep();
    }

    void UpdateTextBoxes()
    {
        //--------------------- Level complete and UI.
        ScoreTextBox.text = "Coin score: " + Score.ToString();
        CompletionTextBox.text = "Objective:"; //UI information converted to text string.
        if (LevelComplete == true)
        {
            CompletionTextBox.text = "Objective: COMPLETE!";
        }
        else
        {
            CompletionTextBox.text = "Objective: GET COINS!";
        }
        TimerTextBox.text = $"Time left: {(int)Timer}"; //interpolation method of the above and below.
         //--------------------- Game over logic such as instantiation and text change.
        if (Timer <= 0 && Score != 3)
        {

            TimerTextBox.text = "GOD DAMN IT!!!!!";
            GameObject.Instantiate(
                GameObject.Find("Player Ball"),
               new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), Random.Range(-5f, 5f)),
                Quaternion.identity);
            Ballcount = Ballcount + 1;
            PlayerSound.PlayOneShot(gameOverClip);
            if (Ballcount >= 8)
            {
                this.enabled = false;
            }
        }

        // you can do +timer.toString("F0") F0 is telling the computer to turn it into a single point float/no decimal.
    }

    // Update is called once per frame
    void Update()
    {
        MovementControls();
        LevelWrappingHandler();
        TimerFunctionality();
        RespawnHandler();
        ScoreSaveLogic();

    }

    private void ScoreSaveLogic()
    {
        //--------------------- save data (int, strings or floats.)

        if (Keyboard.current.f1Key.wasPressedThisFrame)
        {
            PlayerPrefs.SetInt("PlayerScore", Score); //saves the score.
        }
        if (Keyboard.current.f2Key.wasPressedThisFrame)
        {
            Score = PlayerPrefs.GetInt("PlayerScore");
        }
    }

    private void RespawnHandler()
    {
        if (Keyboard.current.rKey.wasReleasedThisFrame)   //OUR ANTISTUCK tool, teleports X coordinates to 3 
        {
            transform.position = new Vector3(3, 0, 0);
        }
    }

    private void TimerFunctionality()
    {
        Timer = Timer - Time.deltaTime; //frame rate independence, allows for the timer to go down in real seconds
        Timer = Mathf.Clamp(Timer, 0, 60); // cap the countdown to 0
        UpdateTextBoxes();
    }

    private void LevelWrappingHandler()
    {
        //--------------------- wrapping to ensure ball bounces back, right side.

        float bounceSpeed = moveSpeed / 2; //halves the movespeed. higher the number it's divided by, weaker the bounce.
        float rightBoundary = 9.53f; //roughly the edge of the floor
        if (transform.position.x > rightBoundary)
        {
            float newXspeed = -bounceSpeed; //- due to the game treating right as +, so left is negative.
            float maintainZspeed = physicsBody.linearVelocity.z; // this maintains the Z velocity to simulate bouncing at an angle
            physicsBody.linearVelocity = new Vector3(newXspeed, 0, maintainZspeed);
        }

        //--------------------- left side for boundary bouncing.

        float leftBoundary = 1.003f;
        if (transform.position.x < leftBoundary)
        {
            float newXspeed = +bounceSpeed;
            float maintainZspeed = physicsBody.linearVelocity.z;
            physicsBody.linearVelocity = new Vector3(newXspeed, 0, maintainZspeed);
        }

        //--------------------- Z axis wrapping top

        float topBoundary = 9.955f;
        if (transform.position.z > topBoundary)
        {
            float newZspeed = -moveSpeed;
            float maintainXspeed = physicsBody.linearVelocity.x;
            physicsBody.linearVelocity = new Vector3(maintainXspeed, 0, newZspeed);
        }

        //--------------------- Z axis wrapping bottom

        float bottomBoundary = -9.955f;
        if (transform.position.z < bottomBoundary)
        {
            float newZspeed = +moveSpeed;
            float maintainXspeed = physicsBody.linearVelocity.x;
            physicsBody.linearVelocity = new Vector3(maintainXspeed, 0, newZspeed);
        }
    }

    private void MovementControls()
    {
        //--------------------- input system

        float zInput = (Keyboard.current.wKey.value - Keyboard.current.sKey.value);
        float xInput = (Keyboard.current.dKey.value - Keyboard.current.aKey.value);
        //float zInput = (Gamepad.current.leftStick.y.value);
        //float xInput = (Gamepad.current.leftStick.x.value);

        //--------------------- velocity control for physics engine to handle movement

        Vector3 direction = new Vector3(xInput, 0, zInput);
        physicsBody.AddForce(new Vector3(xInput, 0, zInput) * moveSpeed);
    }
}
