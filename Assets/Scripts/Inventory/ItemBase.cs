using System.Collections;
using System.Collections.Generic;
using UnityEditor.XR;
using UnityEngine;

public class ItemBase : ScriptableObject
{
    [SerializeField] string name;
    [SerializeField] string description;
    [SerializeField] Sprite icon;
    [SerializeField] int uses;
    [SerializeField] bool unlimetedUse;

    public string Name => name;
    public string Description => description;
    public Sprite Icon => icon;
    public int Uses => uses;
    public bool UnlimetedUse => unlimetedUse;
}
