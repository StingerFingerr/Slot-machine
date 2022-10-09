using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class SlotMachine : MonoBehaviour
{
    public List<Slot> slots;
    public Animator animator;
    public TextMeshProUGUI scoreText;
    public List<GameObject> vfxGameObjects;

    private static readonly int Launch = Animator.StringToHash("Launch");
    private int _rotatingSlotsCount;
    private List<SlotValue> _slotValues;
    private bool _isCanLaunch = true;
    private const int DefaultPrize = 100;
    
    private void Awake() => 
        _slotValues = new(slots.Count);

    private void OnMouseDown() => 
        StartSlotMachine();

    private void StartSlotMachine()
    {
        if(_isCanLaunch is false)
            return;
        _isCanLaunch = false;
        
        ClearPastResults();
        PlayAnimations();
        LaunchSlots();
    }

    private void LaunchSlots()
    {
        slots.ForEach(x=>x.StartRotating(SlotStopped));
        _rotatingSlotsCount = slots.Count;
    }

    private void SlotStopped(SlotValue value)
    {
        _rotatingSlotsCount--;
        _slotValues.Add(value);
        if (_rotatingSlotsCount <= 0)
            Scoring();
    }

    private void Scoring()
    {
        SetPrizeText(CalculatePrize());
        ShowEffects();
        _isCanLaunch = true;
    }

    private int CalculatePrize()
    {
        var res = GetMostCommonWithCount();

        return res.Item2 switch
        {
            2 => DefaultPrize * 2,
            3 => DefaultPrize * 3,
            4 => DefaultPrize * 4,
            _ => DefaultPrize
        };
    }

    private Tuple<SlotValue, int> GetMostCommonWithCount()
    {
        var group = _slotValues
            .GroupBy(x => x)
            .Select(x => new {Key = x.Key, Count = x.Count()});

        var res = group.FirstOrDefault(x => x.Count == group.Max(y => y.Count));
        return new Tuple<SlotValue, int>(res.Key, res.Count);
    }

    private void SetPrizeText(int prize)
    {
        scoreText.text = $"Your prize: {prize}";
        scoreText.gameObject.SetActive(true);
    }

    private void PlayAnimations() =>
        animator.SetTrigger(Launch);

    private void ClearPastResults()
    {
        scoreText.gameObject.SetActive(false);
        _slotValues.Clear();
    }

    private void ShowEffects() => 
        vfxGameObjects.ForEach(x=>x.SetActive(true));
}