using System;
using System.Collections;
using echo17.EndlessBook;
using UnityEngine;
using UnityEngine.InputSystem;

public class BookInteraction : MonoBehaviour
{
    [Header("Book")]
    [SerializeField] private EndlessBook book;
    [SerializeField] private ProfileBookUI bookUI;
    
    [Header("Camera")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Transform bookViewAnchor;
    [SerializeField] private float cameraTransitionTime = 1f;
    
    [Header("Player")]
    [SerializeField] private GameObject playerCapsule;
    [SerializeField] private MonoBehaviour playerFollowCameraScript; 
    
    [Header("Timings")]
    [SerializeField] private float closedToOpenFrontTime = 0.7f;
    [SerializeField] private float openFrontToOpenMiddleTime = 0.7f;
    [SerializeField] private float pageTurnTime = 0.7f;
    
    // Input
    private InputActionMap playerMap;
    private InputActionMap bookMap;
    private InputAction enterBookAction;
    private InputAction exitBookAction;
    private InputAction openCloseAction;
    private InputAction nextPageAction;
    private InputAction previousPageAction;
    
    // State
    private enum BookMode { OutsideRange, InRange, InBookView }
    private BookMode mode = BookMode.OutsideRange;
    
    private Vector3 cameraOriginalPosition;
    private Quaternion cameraOriginalRotation;
    private Transform cameraOriginalParent;
    
    private void Awake()
    {
        playerMap = InputSystem.actions.FindActionMap("Player");
        bookMap = InputSystem.actions.FindActionMap("Book");
        
        enterBookAction = playerMap.FindAction("Interact");
        exitBookAction = bookMap.FindAction("ExitBook");
        openCloseAction = bookMap.FindAction("OpenCloseBook");
        nextPageAction = bookMap.FindAction("NextPage");
        previousPageAction = bookMap.FindAction("PreviousPage");
        
        bookMap.Disable();
    }
    
    private void Start()
    {
        book.SetState(EndlessBook.StateEnum.ClosedFront, 0f);
        bookUI.Hide();
    }

    private void Update()
    {
        if (mode == BookMode.InRange && enterBookAction.WasPressedThisFrame())
        {
            EnterBookView();
            return;
        }
        
        if (mode == BookMode.InBookView)
        {
            if (exitBookAction.WasPressedThisFrame())
            {
                ExitBookView();
                return;
            }
        
            if (nextPageAction.WasPressedThisFrame())
            {
                ForwardBookState();
            }

            if (previousPageAction.WasPressedThisFrame())
            {
                PreviousBookState();
            }
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            mode = BookMode.InRange;
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && mode == BookMode.InRange)
        {
            mode = BookMode.OutsideRange;
        }
    }
    private void EnterBookView()
    {
        mode = BookMode.InBookView;
        
        // disable player movement and camera
        playerMap.Disable();
        if (playerFollowCameraScript != null) playerFollowCameraScript.enabled = false;
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        bookMap.Enable();
        
        // camera to book view
        StartCoroutine(MoveCameraTo(bookViewAnchor.position, bookViewAnchor.rotation, 
            cameraTransitionTime, onComplete: null));
    }
    
    private void ExitBookView()
    {
        mode = BookMode.InRange;
        
        // Hide UI and close book
        bookUI.Hide();
        book.SetState(EndlessBook.StateEnum.ClosedFront, closedToOpenFrontTime);
        
        bookMap.Disable();
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        StartCoroutine(MoveCameraTo(cameraOriginalPosition, cameraOriginalRotation, 
            cameraTransitionTime, onComplete: () =>
        {
            mainCamera.transform.SetParent(cameraOriginalParent);
            if (playerFollowCameraScript != null) playerFollowCameraScript.enabled = true;
            playerMap.Enable();
        }));
    }
    
    private void ForwardBookState()
    {
        switch (book.CurrentState)
        {
            case EndlessBook.StateEnum.ClosedFront:
                book.SetState(EndlessBook.StateEnum.OpenFront, closedToOpenFrontTime);
                break;
                
            case EndlessBook.StateEnum.OpenFront:
                book.SetState(EndlessBook.StateEnum.OpenMiddle, openFrontToOpenMiddleTime, 
                    OnReachedOpenMiddle);
                break;
                
            case EndlessBook.StateEnum.OpenMiddle:
                break;
        }
    }
    
    private void PreviousBookState()
    {
        switch (book.CurrentState)
        {
            case EndlessBook.StateEnum.OpenMiddle:
                bookUI.Hide();
                book.SetState(EndlessBook.StateEnum.OpenFront, closedToOpenFrontTime);
                break;
                
            case EndlessBook.StateEnum.OpenFront:
                bookUI.Hide();
                book.SetState(EndlessBook.StateEnum.ClosedFront, openFrontToOpenMiddleTime);
                break;
                
            case EndlessBook.StateEnum.ClosedFront:
                break;
        }
    }
    
    private void OnReachedOpenMiddle(EndlessBook.StateEnum from, 
                                      EndlessBook.StateEnum to, 
                                      int pageNumber)
    {
        if (to == EndlessBook.StateEnum.OpenMiddle)
        {
            bookUI.Show();
        }
    }
    
    // private void OnPageTurnComplete(EndlessBook.StateEnum from, 
    //                                  EndlessBook.StateEnum to, 
    //                                  int pageNumber)
    // {
    //     bookUI.Show();
    // }
    
    private IEnumerator MoveCameraTo(Vector3 targetPos, Quaternion targetRot, 
                                      float duration, System.Action onComplete)
    {
        if (cameraOriginalParent == null)
        {
            cameraOriginalParent = mainCamera.transform.parent;
            cameraOriginalPosition = mainCamera.transform.position;
            cameraOriginalRotation = mainCamera.transform.rotation;
        }
        
        mainCamera.transform.SetParent(null);
        
        Vector3 startPos = mainCamera.transform.position;
        Quaternion startRot = mainCamera.transform.rotation;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);
            mainCamera.transform.position = Vector3.Lerp(startPos, targetPos, t);
            mainCamera.transform.rotation = Quaternion.Slerp(startRot, targetRot, t);
            yield return null;
        }
        
        mainCamera.transform.position = targetPos;
        mainCamera.transform.rotation = targetRot;
        
        onComplete?.Invoke();
    }
}