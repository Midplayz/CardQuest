using UnityEngine;

[CreateAssetMenu(fileName = "WatermarkTextData", menuName = "ScriptableObjects/WatermarkTextData", order = 1)]

/*Scriptable object that holds the text to show in the Watermark Text on the dashboard.*/

public class WatermarkTextData : ScriptableObject
{
    public string[] texts;
}