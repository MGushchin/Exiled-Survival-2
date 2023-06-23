using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public interface ILevelSetup
{
    public GameObject selfObject { get; }
    public void InitSetup(Tilemap walkable, Tilemap blockable);

    public MapData Setup(int size, float noise, float decorationsPercent);

    public MapData SetupRandomly(int width, int height);
}
