﻿using UnityEngine;

// Basic data for buildings when saved needed to reconstruct them
[System.Serializable]
public class BuildingSave : WorldObjectSave
{
    public float presentHealth;
    public float yOffset;
    public bool buildingFinished;

    public BuildingSave(float health, float yPos, bool finished, string name, Vector3 position, Vector3 rotation) : base(name, position, rotation)
    {
        presentHealth = health;
        yOffset = yPos;
        buildingFinished = finished;
    }
}
