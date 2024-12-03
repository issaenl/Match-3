using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu (fileName = "World", menuName = "Level")]
public class Level : ScriptableObject
{
    [Header("Board Size")]
    public int width;
    public int height;

    [Header("Starting map")]
    public TileType[] boardLayout;

    [Header("Available elements")]
    public GameObject[] elements;

    [Header("Score goals")]
    public int[] scoreGoals;

    [Header("Game requirements")]
    public EndGameRequirements requirements;
    public BlankGoal[] levelGoals;
}
