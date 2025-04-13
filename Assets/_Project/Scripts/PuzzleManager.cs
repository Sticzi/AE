using UnityEngine;
using System.Collections.Generic;

public class PuzzleManager : MonoBehaviour
{
    [SerializeField] private DoorController puzzleDoor;

    [Header("Candle Settings")]
    [SerializeField] private List<Candle> candles;
    [SerializeField] private GameObject objectToDisable;
    [SerializeField] private GameObject[] objectToEnableOnExtinguishedCandles;
    [SerializeField] private GameObject[] objectToEnableOnPuzzleSolved;

    [Header("Pedestal Settings")]
    [SerializeField] private List<Pedestal> pedestals;
    //[SerializeField] private DoorController puzzleDoor;

    private bool candlesSolved;
    private bool pedestalsSolved;

    private void Start()
    {
        // Zarejestruj wszystkie �wieczniki i piedesta�y
        foreach (var candle in candles)
        {
            candle.OnExtinguished += CheckCandles;
        }

        foreach (var pedestal in pedestals)
        {
            pedestal.OnItemInserted += CheckPedestals;
        }
    }

    private void CheckCandles()
    {
        if (candlesSolved) return;

        foreach (var candle in candles)
        {
            if (candle.isLit)
            {
                return; // Znajd� zapalon� �wiec�
            }
        }
        //jak nie ma zapalonych �wiec to pierwsza czesc zagadki zosta�a zrobiona i odblokowuje nast�pne wskaz�wki
        candlesSolved = true;
        objectToDisable.SetActive(false);
        foreach (GameObject item in objectToEnableOnExtinguishedCandles)
        {
            item.SetActive(true);
        }
    }

    private void CheckPedestals()
    {
        if (pedestalsSolved) return;

        foreach (var pedestal in pedestals)
        {
            if (!pedestal.HasItem)
            {
                return; // Znajd� pusty piedesta�
            }
        }
        //je�li wszystkie piedesta�y zosta�y aktywowane to puzzle zosta� zrobiony i otwieraj� si� drzwi do przej�cia do nast�pnego poziomu
        
        pedestalsSolved = true;
        PuzzleSolved();
    }

    private void PuzzleSolved()
    {
        foreach (GameObject item in objectToEnableOnPuzzleSolved)
        {
            item.SetActive(true);
        }
        puzzleDoor.OpenDoor();
    }

    private void OnDestroy()
    {
        // Wyrejestruj eventy
        foreach (var candle in candles)
        {
            if (candle != null)
                candle.OnExtinguished -= CheckCandles;
        }

        foreach (var pedestal in pedestals)
        {
            if (pedestal != null)
                pedestal.OnItemInserted -= CheckPedestals;
        }
    }
}