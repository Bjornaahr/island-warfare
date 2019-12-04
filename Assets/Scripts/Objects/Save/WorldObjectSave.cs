﻿using UnityEngine;

[System.Serializable]
public class WorldObjectSave
{
    public float[] position;
    public float[] rotation;

    public WorldObjectSave(Vector3 pos, Vector3 rot)
    {
        position = new float[] { pos.x, pos.y, pos.z };
        rotation = new float[] { rot.x, rot.y, rot.z };
    }
}
