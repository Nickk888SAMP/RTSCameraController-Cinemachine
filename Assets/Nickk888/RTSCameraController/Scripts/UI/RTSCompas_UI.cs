using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTSCompas_UI : MonoBehaviour
{
    [Header("Pivot Point")]
    [SerializeField] private RectTransform pivotRectTransform;
    [Header("Cardinal Points")]
    [SerializeField] private List<CardinalPoints> cardinalPoints;
    [Header("Settings"), Range(0, 75)]
    [SerializeField] private float distanceFromCenter = 50f;


    [Serializable]
    private class CardinalPoints 
    {
        public RectTransform rectTransform;
        public float angle;
    }

    private void LateUpdate()
    {
        float currentPivotAngle = GetPivotAngle();
        foreach(var cardinalPoint in cardinalPoints)
        {
            if (cardinalPoint.rectTransform == null) continue;
            (float x, float y) = CalculateAngleToPosition(currentPivotAngle, cardinalPoint.angle, distanceFromCenter);
            cardinalPoint.rectTransform.localPosition = new Vector2(x, y);
        }
        
    }

    private (float, float) CalculateAngleToPosition(float angle, float angleOffset, float distance)
    {
        float radians = (angle + angleOffset) * Mathf.Deg2Rad;
        return (
            Mathf.Sin(radians) * distance, 
            Mathf.Cos(radians) * distance
            );
    }

    private float GetPivotAngle() => -pivotRectTransform.eulerAngles.z;
}
