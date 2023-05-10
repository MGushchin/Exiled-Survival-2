using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerInputControl : MonoBehaviour
{
    [HideInInspector]
    public UnityEvent<Vector3> OnMovementClick = new UnityEvent<Vector3>();
    [HideInInspector]
    public UnityEvent<Vector3> OnLookDirectionChange = new UnityEvent<Vector3>();
    [HideInInspector]
    public UnityEvent<Vector3, int> OnSkillClick = new UnityEvent<Vector3, int>();
    [HideInInspector]
    public UnityEvent<int> OnAutocastChange = new UnityEvent<int>();
    [HideInInspector]
    public UnityEvent OnOpenMenu = new UnityEvent();
    public UnityEvent OnOpenLevelUp = new UnityEvent(); //Переписать привязчку через UnityEvents

    private PlayerInput input;
    public KeyCode[] SkillsButtons = new KeyCode[5] { KeyCode.Mouse0, KeyCode.Mouse1, KeyCode.Q, KeyCode.E, KeyCode.R};
    [SerializeField]
    private KeyCode autoCastButton = KeyCode.LeftAlt;
    [SerializeField]
    private KeyCode menuButton = KeyCode.Escape;
    [SerializeField]
    public KeyCode LevelUpButton = KeyCode.Tab;

    private void Awake()
    {
        input = new PlayerInput();
    }

    private void OnEnable()
    {
        input.Enable();
    }

    private void OnDisable()
    {
        input.Disable();
    }

    private void Update()
    {
        Vector3 mousePosition = Input.mousePosition; 
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition); 
        mousePosition.z = 0; 
        //Main logic
        movementLogic();
        mouseLogic(mousePosition);
        skillActivationsLogic(mousePosition);
        autoCastButtonsLogic();
        if (Input.GetKeyDown(menuButton))
            OnOpenMenu.Invoke();
        if (Input.GetKeyDown(LevelUpButton))
            OnOpenLevelUp.Invoke();
    }

    private void movementLogic()
    {
        Vector3 move = new Vector3(0, 0, 0);
        if (Input.GetKey(KeyCode.W))
        {
            move.y += 1;
        }
        if (Input.GetKey(KeyCode.S))
        {
            move.y -= 1;
        }
        if (Input.GetKey(KeyCode.D))
        {
            move.x += 1;
        }
        if (Input.GetKey(KeyCode.A))
        {
            move.x -= 1;
        }
        OnMovementClick.Invoke(move);
    }

    private void mouseLogic(Vector3 mousePosition)
    {
        OnLookDirectionChange.Invoke(mousePosition);
    }

    private void skillActivationsLogic(Vector3 mousePosition)
    {
        if (!Input.GetKey(autoCastButton))
        {
            if (Input.GetKey(SkillsButtons[0]))
            {
                OnSkillClick.Invoke(mousePosition, 0);
            }
            if (Input.GetKey(SkillsButtons[1]))
            {
                OnSkillClick.Invoke(mousePosition, 1);
            }
            if (Input.GetKey(SkillsButtons[2]))
            {
                OnSkillClick.Invoke(mousePosition, 2);
            }
            if (Input.GetKey(SkillsButtons[3]))
            {
                OnSkillClick.Invoke(mousePosition, 3);
            }
            if (Input.GetKey(SkillsButtons[4]))
            {
                OnSkillClick.Invoke(mousePosition, 4);
            }
        }
    }

    private void autoCastButtonsLogic()
    {
        if(Input.GetKey(autoCastButton))
        {
            if (Input.GetKeyDown(SkillsButtons[0]))
            {
                OnAutocastChange.Invoke(0);
            } else
            if (Input.GetKeyDown(SkillsButtons[1]))
            {
                OnAutocastChange.Invoke(1);
            }
            else
            if (Input.GetKeyDown(SkillsButtons[2]))
            {
                OnAutocastChange.Invoke(2);
            }
            else
            if (Input.GetKeyDown(SkillsButtons[3]))
            {
                OnAutocastChange.Invoke(3);
            }
            if (Input.GetKeyDown(SkillsButtons[4]))
            {
                OnAutocastChange.Invoke(4);
            }
        }
    }
}
