using System;
using System.Collections;
using UnityEngine;

public class Slot: MonoBehaviour
{
    public Transform raw;

    public float topRawY = 2.65f;
    public float bottomRawY = -2.75f;
    public float distBetweenElementsY = .45f;

    public float spinTime = 3.5f;
    public float spinSpeed = 5f;
    public float correctSpeed = 10f;
    public AnimationCurve slowdown;

    private Action<SlotValue> _onSlotStopped;
    public void StartRotating(Action<SlotValue> callback)
    {
        _onSlotStopped = callback;
        
        StartCoroutine(LaunchSlot());
    }

    private IEnumerator LaunchSlot()
    {
        yield return RotateSlot();
        yield return CorrectSlotRotation();

        _onSlotStopped?.Invoke(GetSlotValue());
        _onSlotStopped = null;
    }

    private IEnumerator RotateSlot()
    {
        float currentY = GetRawYPosition();
        float newSpeed;
        float timer = spinTime;
        
        while (timer >= 0)
        {
            timer -= Time.deltaTime;
            newSpeed = spinSpeed * slowdown.Evaluate(1 - timer / spinTime);
            currentY -= Time.deltaTime * newSpeed;

            SetRawInYPosition(currentY);
            
            if (RawInLowerPosition())
            {
                currentY = topRawY;
                SetRawInUpperPosition();
            }
            yield return null;
        }
    }

    private IEnumerator CorrectSlotRotation()
    {
        float near = GetNearestPos();
        float currentY = GetRawYPosition();
        float threshold = .001f;

        while (Mathf.Abs(currentY - near) > threshold)
        {
            currentY = Mathf.Lerp(currentY, near, Time.deltaTime * correctSpeed);

            SetRawInYPosition(currentY);
            yield return null;
        }

        SetRawInYPosition(near);
    }

    private float GetNearestPos()
    {
        float currentY = GetRawYPosition();
        float low = topRawY;

        while (low > currentY)
            low -= distBetweenElementsY;

        if (low == bottomRawY)
        {
            SetRawInYPosition(currentY + (topRawY - bottomRawY));
            return topRawY;
        }

        float top = low + distBetweenElementsY;

        if (MathF.Abs(currentY - low) < Mathf.Abs(currentY - top))
            return low;
        else
            return top;
    }

    private bool RawInLowerPosition() => 
        raw.localPosition.y <= bottomRawY;

    private void SetRawInUpperPosition() => 
        raw.localPosition = new Vector3(raw.localPosition.x, topRawY);

    private void SetRawInYPosition(float posY) => 
        raw.localPosition = new Vector3(raw.localPosition.x, posY);

    private float GetRawYPosition() => 
        raw.localPosition.y;

    private SlotValue GetSlotValue()
    {
        int id = (int) ((topRawY - raw.localPosition.y) / distBetweenElementsY + 1);
        
        return GetSlotValueByID(id);
    }

    private SlotValue GetSlotValueByID(int id)
    {
        return id switch
        {
            1 => SlotValue.Diamond,
            2 => SlotValue.Clover,
            3 => SlotValue.RedApple,
            4 => SlotValue.Crown,
            5 => SlotValue.Heart,
            6 => SlotValue.Horseshoe,
            7 => SlotValue.Grape,
            8 => SlotValue.Bar,
            9 => SlotValue.Watermelon,
            10 => SlotValue.Cherry,
            11 => SlotValue.Ruby,
            12 => SlotValue.OrangeApple,
            _ => SlotValue.None
        };
    }
}