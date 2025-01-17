﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Scriptable object for military units so we can create them fast 
[CreateAssetMenu]
public class MilitaryUnit : ScriptableObject
{
    public new string name;
    public string decription;
    public Sprite img;
    public int attackPower, defencePower, supplyPower, cost;
}
