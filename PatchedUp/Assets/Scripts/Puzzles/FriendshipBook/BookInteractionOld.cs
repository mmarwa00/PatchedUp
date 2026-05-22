using System;
using echo17.EndlessBook;
using UnityEngine;
using UnityEngine.InputSystem;

public class BookInteractionOld : MonoBehaviour
{
    [SerializeField] private EndlessBook book;

    private InputActionMap playerMap;
    private InputActionMap bookMap;
    private InputAction openCloseBookAction;
    private InputAction nextPageAction;
    private InputAction previousPageAction;
    private bool inRange;

    private void Awake()
    {
        playerMap = InputSystem.actions.FindActionMap("Player");
        bookMap = InputSystem.actions.FindActionMap("Book");
        openCloseBookAction = bookMap.FindAction("OpenCloseBook");
        nextPageAction = bookMap.FindAction("NextPage");
        previousPageAction = bookMap.FindAction("PreviousPage");

        openCloseBookAction.performed += OnOpenClose;
        nextPageAction.performed += OnNextPage;
        previousPageAction.performed += OnPreviousPage;
        
        bookMap.Disable();
    }

    void Start()
    {
        book.SetState(EndlessBook.StateEnum.ClosedFront, 0.7f);
    }

    private void OnOpenClose(InputAction.CallbackContext ctx)
    {
        if (book.CurrentState == EndlessBook.StateEnum.ClosedFront)
        {
            playerMap.Disable();
            book.SetState(EndlessBook.StateEnum.OpenMiddle, 0.7f);
        }
        else
        {
            playerMap.Enable();
            book.SetState(EndlessBook.StateEnum.ClosedFront, 0.7f);
        }
            
    }

    private void OnNextPage(InputAction.CallbackContext ctx)
    {
        book.TurnForward(0.7f);
    }

    private void OnPreviousPage(InputAction.CallbackContext ctx)
    {
        book.TurnBackward(0.7f);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            bookMap.Enable();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            bookMap.Disable();
        }
    }
}
