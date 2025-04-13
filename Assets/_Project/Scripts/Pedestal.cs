using UnityEngine;
using DG.Tweening;
using TMPro;

public class Pedestal : Interactable
{
    [Header("Settings")]
    [SerializeField] private InteractableTypes acceptedType = InteractableTypes.Any;
    [SerializeField] private bool allowOnlySpecificItems = false;
    public Transform Socket;
    public Vector3 targetRotation = new(0, 90, 0);
    public float placementTime = 0.7f;    

    public System.Action OnItemInserted;
    public bool HasItem => insertedItem != null;

    private Interactable insertedItem;

    [Header("Effects")]
    public ParticleSystem sparkEffect;

    public override void Interact(PlayerController player)
    {
        if (player.interactableInHand is Interactable heldItem)
        {
            if (IsItemAccepted(heldItem))
            {
                InsertItem(heldItem, player);
            }
            else
            {
                HandleWrongItem(heldItem, player.GetComponent<InteractionDetection>().InteractionTextField);
            }
        }
    }

    private bool IsItemAccepted(Interactable item)
    {
        if (!allowOnlySpecificItems) return true;
        return item.GetInteractableType() == acceptedType;
    }

    private void HandleWrongItem(Interactable item, TextMeshProUGUI interactionTextField)
    {
        string wrongText = item.name + " " + GetInteractionText();

        // Upewnij siê, ¿e tekst ma pe³n¹ przezroczystoœæ na start
        interactionTextField.alpha = 0;
        interactionTextField.text = wrongText;

        Sequence seq = DOTween.Sequence();
        seq.Append(interactionTextField.DOFade(1f, 0.5f)) // Fade in
           .AppendInterval(2f)                             // Czekaj 2 sekundy
           .Append(interactionTextField.DOFade(0f, 0.5f)) // Fade out
           .OnComplete(() => interactionTextField.text = ""); // Wyzeruj tekst
    }

    private void InsertItem(Interactable item, PlayerController player)
    {
        insertedItem = item;
        OnItemInserted?.Invoke();
        player.interactableInHand = null;

        insertedItem.ResetTransformToOriginal();
        insertedItem.SetLayerRecursively(insertedItem.gameObject, LayerMask.NameToLayer("Interactable"));

        //po wsadzeniu miecza zmieniam layer ¿eby ju¿ nie podœwietla³o siê przy najechaniu myszk¹
        SetLayerRecursively(gameObject, LayerMask.NameToLayer("Default"));

        insertedItem.transform.SetParent(Socket);
        // Sekwencja animacji
        Sequence insertionSequence = DOTween.Sequence();

        // Faza 1: Uniesienie z rotacj¹
        insertionSequence.Append(
            insertedItem.transform.DOLocalMove(new Vector3(0, 1.2f, 0), placementTime * 0.4f)
                .SetEase(Ease.OutQuad)
        );
        insertionSequence.Join(
            insertedItem.transform.DOLocalRotate(targetRotation, placementTime * 0.4f)
                .SetEase(Ease.OutQuad)
        );

        insertionSequence.AppendInterval(0.2f); // Pauza 0.2s miêdzy fazami

        // Faza 2: w³o¿enie do socketu
        insertionSequence.Append(
            insertedItem.transform.DOLocalMove(Vector3.zero, placementTime * 0.6f)
                .SetEase(Ease.OutQuart)
        );

    }

    private void PlayPlacementEffects()
    {
        if (sparkEffect != null)
            sparkEffect.Play();

    }
}