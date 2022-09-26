using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public static Player instance;

    [SerializeField] private float staminaBarSize;
    [SerializeField] private Image staminaFillImage;

    private float staminaAmount;
    private bool isSprinting;
    private bool sprintKeyDown;
    private bool cantSprint;
    private bool hasLiftedSprintKey;


    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        Cursor.lockState = CursorLockMode.Locked;
        staminaAmount = staminaBarSize;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale == 0)
        {
            if (Keyboard.current.eKey.wasPressedThisFrame)
                MenuManager.instance.startGame();
            else if (Keyboard.current.rKey.wasPressedThisFrame)
                MenuManager.instance.restart();
        }

        Cursor.visible = true;

        if (isSprinting)
            staminaAmount -= Time.deltaTime;
        else if (staminaAmount < staminaBarSize)
            staminaAmount += Time.deltaTime;

        if (staminaAmount < 0)
            cantSprint = true;
        else if (cantSprint && staminaAmount > staminaBarSize / 3f && hasLiftedSprintKey)
            cantSprint = false;

        staminaAmount = Mathf.Clamp(staminaAmount, 0, staminaBarSize);

        staminaFillImage.fillAmount = staminaAmount / staminaBarSize;
    }

    public void setIsSprinting(bool on)
    {
        isSprinting = on;
    }

    public void setIsSprintKeyDown(bool on)
    {
        //if (on)
        //    AudioManager.incTheme();

        if (cantSprint && sprintKeyDown && !on)
            hasLiftedSprintKey = true;
        else if (on)
            hasLiftedSprintKey = false;

        sprintKeyDown = on;
    }

    public bool getIsSprinting()
    {
        return isSprinting;
    }

    public bool canSprint()
    {
        return staminaAmount > 0 && !cantSprint;
    }
}
