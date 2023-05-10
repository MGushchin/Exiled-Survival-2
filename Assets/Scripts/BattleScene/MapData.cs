using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapData
{
    private bool[,] matrix;
    private int xSize = 0;
    private int ySize = 0;
    private int xOffset = 0;
    private int yOffset = 0;

    public MapData(int xSize, int ySize, int xOffset, int yOffset)
    {
        this.xSize = xSize;
        this.ySize = ySize;
        this.xOffset = xOffset;
        this.yOffset = yOffset;
        matrix = new bool[xSize, ySize];
    }

    public void SetPoint(int x, int y, bool value)
    {
        if (x - xOffset < xSize && y - yOffset < ySize)
        {
            matrix[x - xOffset, y - yOffset] = value;
        }
    }

    public bool GetPoint(int x, int y)
    {
        if (x - xOffset < xSize && y - yOffset < ySize)
        {
            return matrix[x - xOffset, y - yOffset];
        }
        else
            return false;
    }

    public bool ContainEmpty(int x, int y)
    {
        if (x - xOffset < xSize && y - yOffset < ySize)
        {
            if (matrix[x - xOffset, y - yOffset] == true)
                return true;
        }
        return false;
    }

    public Vector3 GetCloser(int x, int y)
    {
        //int processedX = Mathf.Clamp(x - xOffset, 0, xSize);
        //int processedY = Mathf.Clamp(y - yOffset, 0, ySize);
        int processedX = Mathf.Clamp(x - xOffset, 0, xSize);
        int processedY = Mathf.Clamp(y - yOffset, 0, ySize);
        return new Vector3(processedX + xOffset, processedY + yOffset);
    }
}
