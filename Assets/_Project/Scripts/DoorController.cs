using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;

public class DoorController : MonoBehaviour
{
    [SerializeField] private Vector3 openRotation;
    [SerializeField] private float openDuration = 2f;
    [SerializeField] private Image winScreen;
    [SerializeField] private TextMeshProUGUI tmp;
    [SerializeField] private Transform doorTransform;
    public void OpenDoor()
    {
        doorTransform.DORotate(openRotation, openDuration)
            .SetEase(Ease.OutBack);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            
            Sequence seq = DOTween.Sequence();

            seq.Append(winScreen.DOFade(1f, 1f).SetEase(Ease.Linear)) // Fade-in winScreen
               .Append(tmp.DOFade(1f, 1f))                            // Potem fade-in tekstu
               .AppendInterval(3f)                                   // Poczekaj 5 sekund
               .AppendCallback(() =>
               {
                   Application.Quit(); // Zamknij grê
#if UNITY_EDITOR
               UnityEditor.EditorApplication.isPlaying = false; // Dzia³a w edytorze
#endif
           });
        }


    }

}