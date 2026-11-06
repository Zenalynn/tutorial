using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControllerTask7 : MonoBehaviour
{
    private int lives = 3;

    private void OnGUI() //manually drawing on lives on screen. uncommon, but good for learning iteration.
    {
        // this is a reference to the same texture the spriterenderer needs to draw the player's texture.
        Texture livesTexture = GetComponent<SpriteRenderer>().sprite.texture;//
        int repetitions = 0;//just an integer we've named to resemble our repetition counter.
        do
        {
            Rect drawlocation = new Rect(x: 1700 - 40 * repetitions, y: 900, width: 30, height: 30);
            GUI.DrawTexture(drawlocation, livesTexture); //asks for location, which we shortened into a rectangle variable and our texture variable
            repetitions = repetitions + 1; //counts up our iteration counter                          
        } while (repetitions < 3); //less that three, duh.
    }

    void Start()
    {
        
    }

    void Update()
    {
    }
}
