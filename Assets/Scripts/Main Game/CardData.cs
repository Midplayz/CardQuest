using UnityEngine;

[System.Serializable]

/* This basically stores the sprites and value for each card. I know I could have used a Scriptable Object for this
 but since I already used it once on the dashboard as requested, I wanted to showcase that I know this method too! */
public class CardData
{
    public Sprite sprite;
    public int value;
}