using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterImage
{
    public Sprite sprite; // The character sprite
    public int associatedValue; // The associated integer value

    public CharacterImage(Sprite sprite, int associatedValue)
    {
        this.sprite = sprite;
        this.associatedValue = associatedValue;
    }
}
