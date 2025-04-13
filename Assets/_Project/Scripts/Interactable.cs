using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    [Header("Interactable Settings")]
    [SerializeField] private string interactionText = "";
    [SerializeField] private Material highlightMaterial;
    private Renderer objectRenderer;
    private GameObject tooltipE;
    private GameObject tooltipR;
    public bool canBeInteracted;

    [Header("Pickup Settings")]
    public bool canBePickedUp;
    public Vector3 positionInHand;
    public Quaternion rotationInHand;
    public Vector3 scaleInHand;

    [Header("Drop Settings")]
    public Vector3 dropPositionOffset = new Vector3(0, 0.1f, 0);
    public bool alignToSurface = true;
    public Quaternion notAlignedDropRotation;

    [Header("Identification")]
    [SerializeField] private InteractableTypes interactableType = InteractableTypes.Any;

    public InteractableTypes GetInteractableType() => interactableType;

    private Material[] originalMaterials;
    private Transform originalParent;
    private Vector3 originalScale;

    protected virtual void Awake()
    {           
        objectRenderer = GetComponent<Renderer>();
        originalMaterials = objectRenderer.materials;
        //wypada�oby napisa� jeszcze sprawdzajke czy obiekt jest trzymany w r�ku i je�li nie to �eby odrazu na odpaleniu sceny da� mu layer "interactable",
        //�eby nie by�o niepotrzebnych problem�w z niedzia�aj�cym poprawnie obiektem
    }

    public virtual void OnHoverEnter()
    {
        //to dodaje highlight do aktualnie aktywnych materia��w, na razie podmienimy po prostu jeden na drugi skoro paczka simple dungeons ju� zapewnia materia� "shiny"
        //Material[] newMaterials = new Material[originalMaterials.Length + 1];
        //originalMaterials.CopyTo(newMaterials, 0);
        //newMaterials[^1] = highlightMaterial;
        //objectRenderer.materials = newMaterials;
        objectRenderer.material = highlightMaterial;
    }       

    public virtual void OnHoverExit()
    {
        objectRenderer.materials = originalMaterials;
    }

    public abstract void Interact(PlayerController player);

    public virtual void PickUp(PlayerController player)
    {
        if (!canBePickedUp)
            return;

        originalParent = transform.parent;
        originalScale = transform.localScale;

        //w przysz�o�ci je�li b�d� jakie� interactable obiekty, kt�re b�d� mia�y wi�cej collider�w mo�e pojawi� sie w tym miejscu
        //potrzeba zapisania tego, kt�re collidery nale�y w��czy� potem przy dropie, narazie obiekty s� proste i takiej potrzeby nie ma
        ToggleColliders(false);

        SetTransformToHand(player);

        player.interactableInHand = this;

        SetLayerRecursively(gameObject, LayerMask.NameToLayer("Hand"));
    }

    private void SetTransformToHand(PlayerController player)
    {
        transform.SetParent(player.handTransform, false);
        transform.localPosition = positionInHand;
        transform.localRotation = rotationInHand;
        transform.localScale = scaleInHand;
    }

    public void ResetTransformToOriginal()
    {
        transform.SetParent(originalParent);
        transform.localScale = originalScale;
    }

    public void Drop(RaycastHit surfaceHit)
    {
        ResetTransformToOriginal();

        if (alignToSurface)
        {
            transform.SetPositionAndRotation(surfaceHit.point + surfaceHit.normal * dropPositionOffset.y, Quaternion.FromToRotation(Vector3.up, surfaceHit.normal));
        }
        else
        {
            transform.SetPositionAndRotation(surfaceHit.point + dropPositionOffset, notAlignedDropRotation);
        }
        SetLayerRecursively(gameObject, LayerMask.NameToLayer("Interactable"));
        ToggleColliders(true);
    }

    private void ToggleColliders(bool state)
    {
        foreach (Collider collider in GetComponents<Collider>())
        {
            collider.enabled = state;
        }
    }

    public void SetLayerRecursively(GameObject obj, int newLayer)
    {
        if (obj == null) return;

        obj.layer = newLayer;

        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }

    public string GetInteractionText() => interactionText;
}