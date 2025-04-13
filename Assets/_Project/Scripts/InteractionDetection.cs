using UnityEngine;
using TMPro;


public class InteractionDetection : MonoBehaviour
{
    [Header("General Settings")]
    [SerializeField] private float checkRate = 0.1f;
    [SerializeField] private float maxDistance = 3f;
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private GameObject tooltipE;
    [SerializeField] private GameObject tooltipR;
    [SerializeField] private GameObject tooltipHandOccupied;
    [SerializeField] private GameObject tooltipDrop;
    public TextMeshProUGUI InteractionTextField;
    private bool tooltipsShown = false;

    [Header("Drop Settings")]
    [SerializeField] private LayerMask dropSurfaceLayer;
    [SerializeField] private float maxSurfaceAngle = 45f;

    private Interactable currentHoveredInteractable;
    private Camera playerCamera;
    private PlayerController playerController;

    
    public RaycastHit CurrentSurfaceHit { get; private set; }
    public bool IsValidDropSurface { get; private set; }
    public Interactable CurrentHoveredInteractable => currentHoveredInteractable;

    private void Start()
    {
        playerController = GetComponent<PlayerController>();
        playerCamera = GetComponentInChildren<Camera>();
        InvokeRepeating(nameof(CheckForInteractables), 0f, checkRate);
        InvokeRepeating(nameof(CheckForDropSurface), 0f, checkRate);
    }

    //private void Update()
    //{
    //    CheckForDropSurface();
    //}

    private void CheckForInteractables()
    {
        Ray ray = new(playerCamera.transform.position, playerCamera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, interactableLayer))
        {
            HandleNewInteractable(hit);
        }
        else
        {
            ClearCurrentInteractable();
        }
    }

    private void CheckForDropSurface()
    {
        Ray ray = new(playerCamera.transform.position, playerCamera.transform.forward);
        IsValidDropSurface = Physics.Raycast(ray, out RaycastHit hit, maxDistance, dropSurfaceLayer);

        if (IsValidDropSurface)
        {
            //jeœli trzymasz coœ w rêce, patrzysz siê na prost¹ powierzchnie i inne tooltipy nie bêd¹ ci zas³aniay to wyœwietl drop tooltip
            if(playerController.interactableInHand&&!tooltipsShown)
            {
                tooltipDrop.SetActive(true);
            }                      
            CurrentSurfaceHit = hit;
            float surfaceAngle = Vector3.Angle(hit.normal, Vector3.up);
            IsValidDropSurface = surfaceAngle <= maxSurfaceAngle;
        }
        else
        {
            tooltipDrop.SetActive(false);
        }
    }

    private void HandleNewInteractable(RaycastHit hit)
    {
        Interactable newInteractable = hit.collider.GetComponent<Interactable>();
        if (newInteractable == currentHoveredInteractable) return;

        ClearCurrentInteractable();

        currentHoveredInteractable = newInteractable;
        if(currentHoveredInteractable)
        {
            //wyœwietl tooltip, odnoœnie tego co mo¿esz zrobiæ z danym przedmiotem
            ShowToolip(currentHoveredInteractable);
            currentHoveredInteractable.OnHoverEnter();
        }
    }

    protected virtual void ShowToolip(Interactable currentInteractable)
    {
        tooltipDrop.SetActive(false);
        tooltipsShown = true;
        if (currentInteractable.canBeInteracted)
        {
            tooltipE.SetActive(true);
        }
        if (currentInteractable.canBePickedUp)
        {
            if(playerController.interactableInHand)
            {
                tooltipHandOccupied.SetActive(true);
            }
            else
            {
                tooltipR.SetActive(true);
            }            
        }
    }

    protected virtual void HideTooltip()
    {
        tooltipE.SetActive(false);
        tooltipR.SetActive(false);
        tooltipHandOccupied.SetActive(false);   
        tooltipsShown = false;
    }


    private void ClearCurrentInteractable()
    {
        if(currentHoveredInteractable)
        {
            HideTooltip();
            currentHoveredInteractable.OnHoverExit();
        }
        currentHoveredInteractable = null;
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        Gizmos.color = currentHoveredInteractable ? Color.green : Color.red;
        Vector3 rayEnd = playerCamera.transform.position + playerCamera.transform.forward * maxDistance;
        Gizmos.DrawLine(playerCamera.transform.position, rayEnd);
    }
}