using System;
using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;

public class Enemy
{
    EnemyBase _base;
    private int level;

    public Enemy(EnemyBase eBase, int eLevel)
    {
        _base = eBase;
        level = eLevel;
    }

    public int Attack
    {
        get { return Mathf.FloorToInt((_base.BaseAttack * level) / 100f + 5); }
    }

    public int Defense
    {
        get { return Mathf.FloorToInt((_base.BaseDefense * level) / 100f + 5); }

    }

    public int MaxHP
    {
        get { return Mathf.FloorToInt((_base.MaxHP * level) / 100f + 10); }
    }

    public int Speed
    {
        get { return Mathf.FloorToInt((_base.Speed * level) / 100f + 5); }

    }
}
