using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public enum GameState
{
    wait,
    move,
    win,
    lose,
    pause
}

public enum TileKind
{
    Breakable,
    Blank,
    Normal,
}

[System.Serializable]
public class TileType
{
    public int x;
    public int y;
    public TileKind tileKind;
}

[System.Serializable]
public class MatchType
{
    public int type;
    public string color;
}

public class Board : MonoBehaviour
{
    public int level;
    public World world;

    public GameState currentState = GameState.move;

    public int width;
    public int height;
    public int offSet;

    public GameObject tilePrefab;
    public GameObject breakableTilePrefab;
    public GameObject[] dots;
    public GameObject[,] allDots;
    public GameObject destroyEffect;

    public MatchType matchType;
    public Element currentElement;
    public TileType[] boardLayout;
    public int basePieceValue = 20;
    public float refillDelay = 0.5f;
    public int[] scoreGoals;
    private GoalManager goalManager;
    private SoundManager soundManager;
    private bool[,] blankSpaces;
    private BackgroundTile[,] breakableTiles;
    private FindMatches findMatches;
    private ScoreManager scoreManager;
    private int streakValue = 1;

    private void Awake()
    {
        if (PlayerPrefs.HasKey("Current level"))
        {
            level = PlayerPrefs.GetInt("Current level");
        }
        if (world != null)
        {
            if (level < world.levels.Length)
            {
                if (world.levels[level] != null)
                {
                    width = world.levels[level].width;
                    height = world.levels[level].height;
                    dots = world.levels[level].elements;
                    boardLayout = world.levels[level].boardLayout;
                    scoreGoals = world.levels[level].scoreGoals;
                }
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        goalManager = FindObjectOfType<GoalManager>();
        soundManager = FindObjectOfType<SoundManager>();
        scoreManager = FindObjectOfType<ScoreManager>();
        breakableTiles = new BackgroundTile[width, height];
        findMatches = FindObjectOfType<FindMatches>();
        blankSpaces = new bool[width, height];
        allDots =  new GameObject[width, height];
        SetUp();
        currentState = GameState.pause;
    }

    public void GenerateBlankSpaces()
    {
        for (int i = 0; i < boardLayout.Length; i++)
        {
            if(boardLayout[i].tileKind == TileKind.Blank)
            {
                blankSpaces[boardLayout[i].x, boardLayout[i].y] = true;
            }
        }
    }

    public void GenerateBreakableTiles()
    {
        for (int i = 0; i < boardLayout.Length; i++)
        {
            if (boardLayout[i].tileKind == TileKind.Breakable)
            {
                Vector2 tempPosition = new Vector2(boardLayout[i].x, boardLayout[i].y);
                GameObject tile = Instantiate(breakableTilePrefab, tempPosition, Quaternion.identity);
                breakableTiles[boardLayout[i].x, boardLayout[i].y] = tile.GetComponent<BackgroundTile>();
            }
        }
    }

    private void SetUp()
    {
        GenerateBreakableTiles();
        GenerateBlankSpaces();
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (!blankSpaces[i, j])
                {
                    Vector2 tempPosition = new Vector2(i, j + offSet);
                    GameObject backgroundTile = Instantiate(tilePrefab, tempPosition, Quaternion.identity) as GameObject;
                    backgroundTile.transform.parent = this.transform;
                    backgroundTile.name = "( " + i + ", " + j + " )";
                    int dotToUse = Random.Range(0, dots.Length);
                    int maxIterations = 0;
                    while (MatchesAt(i, j, dots[dotToUse]) && maxIterations < 100)
                    {
                        dotToUse = Random.Range(0, dots.Length);
                        maxIterations++;
                    }
                    maxIterations = 0;
                    GameObject dot = Instantiate(dots[dotToUse], tempPosition, Quaternion.identity);
                    dot.GetComponent<Element>().row = j;
                    dot.GetComponent<Element>().column = i;
                    dot.transform.parent = this.transform;
                    dot.name = "( " + i + ", " + j + " )";
                    allDots[i, j] = dot;
                }
            }
        }
    }

    private bool MatchesAt(int column, int row, GameObject piece)
    {
        if (column > 1 && row > 1)
        {
            if (allDots[column - 1, row] != null && allDots[column - 2, row] != null)
            {
                if (allDots[column - 1, row].tag == piece.tag && allDots[column - 2, row].tag == piece.tag)
                {
                    return true;
                }
            }
            if (allDots[column, row - 1] != null && allDots[column, row - 2] != null)
            {
                if (allDots[column, row - 1].tag == piece.tag && allDots[column, row - 2].tag == piece.tag)
                {
                    return true;
                }
            }
        }
        else if(column <= 1 || row <= 1)
        {
            if(row > 1)
            {
                if (allDots[column, row - 1] != null && allDots[column, row - 2] != null)
                {
                    if (allDots[column, row - 1].tag == piece.tag && allDots[column, row - 2].tag == piece.tag)
                    {
                        return true;
                    }
                }
            }
            if (column > 1)
            {
                if (allDots[column - 1, row] != null && allDots[column - 2, row] != null)
                {
                    if (allDots[column - 1, row].tag == piece.tag && allDots[column - 2, row].tag == piece.tag)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private MatchType FindTypeOfBomb()
    {
        List<GameObject> matchCopy = findMatches.currentMatches as List<GameObject>;
        matchType.type = 0;
        matchType.color = "";
        for(int i = 0; i < matchCopy.Count; i++)
        {
            Element thisElement = matchCopy[i].GetComponent<Element>();
            string color = matchCopy[i].tag;
            int column = thisElement.column;
            int row = thisElement.row;
            int columnMatch = 0;
            int rowMatch = 0;
            for (int j = 0; j < matchCopy.Count; j++)
            {
                Element nextElement = matchCopy[j].GetComponent<Element>();
                if(nextElement == thisElement)
                {
                    continue;
                }
                if(nextElement.column == thisElement.column && nextElement.tag == color)
                {
                    columnMatch++;
                }
                if (nextElement.row == thisElement.row && nextElement.tag == color)
                {
                    rowMatch++;
                }
            }
            if(columnMatch == 4 || rowMatch == 4)
            {
                matchType.type = 1;
                matchType.color = color;
                return matchType;
            }
            else if(columnMatch == 2 && rowMatch == 2)
            {
                matchType.type = 2;
                matchType.color = color;
                return matchType;
            }
            else if(columnMatch == 3 || rowMatch == 3)
            {
                matchType.type = 3;
                matchType.color = color;
                return matchType;
            }
        }
        matchType.type = 0;
        matchType.color = "";
        return matchType;
    }

    private void CheckToMakeBombs()
    {
        if(findMatches.currentMatches.Count > 3)
        {
            MatchType typeOfMatch = FindTypeOfBomb();
            if (typeOfMatch.type == 1)
            {
                if (currentElement != null)
                {
                    if (currentElement.isMatched && currentElement.tag == typeOfMatch.color)
                    {
                        currentElement.isMatched = false;
                        currentElement.MakeColorBomb();

                    }
                    else if (currentElement.otherDot != null)
                    {

                        Element otherDot = currentElement.otherDot.GetComponent<Element>();
                        if (otherDot.isMatched && otherDot.tag == typeOfMatch.color)
                        {
                            otherDot.isMatched = false;
                            otherDot.MakeColorBomb();
                        }

                    }
                }
            }
            else if (typeOfMatch.type == 2)
            {
                if (currentElement != null)
                {
                    if (currentElement.isMatched && currentElement.tag == typeOfMatch.color)
                    {
                        currentElement.isMatched = false;
                        currentElement.MakeZoneBomb();
                    }
                    else if (currentElement.otherDot != null)
                    {
                        Element otherDot = currentElement.otherDot.GetComponent<Element>();
                        if (otherDot.isMatched && otherDot.tag == typeOfMatch.color)
                        {
                            otherDot.isMatched = false;
                            otherDot.MakeZoneBomb();
                        }
                    }
                }
            }
            else if(typeOfMatch.type == 3)
            {
                findMatches.CheckBombs(typeOfMatch);
            }
        }
    }

    private void DestroyMatchesAt(int column, int row)
    {
        if (allDots[column, row].GetComponent<Element>().isMatched)
        {
            if (breakableTiles[column, row] != null)
            {
                breakableTiles[column, row].TakeDamage(1);
                if (breakableTiles[column, row].hitPoints <= 0)
                {
                    breakableTiles[column, row] = null;
                }
            }
            if(goalManager != null)
            {
                goalManager.CompareGoal(allDots[column, row].tag.ToString());
                goalManager.UpdateGoals();
            }
            if(soundManager != null)
            {
                soundManager.PlayRandomNoise();
            }
            GameObject particle = Instantiate(destroyEffect, allDots[column, row].transform.position, Quaternion.identity);
            Destroy(particle, .5f);
            Destroy(allDots[column, row]);
            scoreManager.IncreaseScore(basePieceValue * streakValue);
            allDots[column, row] = null;
        }
    }

    public void DestroyMatches()
    {
        if (findMatches.currentMatches.Count >= 4)
        {
            CheckToMakeBombs();
        }
        findMatches.currentMatches.Clear();
        for (int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                if (allDots[i, j] != null)
                {
                    DestroyMatchesAt(i, j);
                }
            }
        }
        StartCoroutine(DecreaseRowCo2());
    }

    private IEnumerator DecreaseRowCo2()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (!blankSpaces[i, j] && allDots[i, j] == null)
                {
                    for(int k = j + 1; k < height; k++)
                    {
                        if(allDots[i, k] != null)
                        {
                            allDots[i, k].GetComponent<Element>().row = j;
                            allDots[i, k] = null;
                            break;
                        }
                    }
                }
            }
        }
        yield return new WaitForSeconds(refillDelay * 0.5f);
        StartCoroutine(FillBoardCo());
    }

    private IEnumerator DecreaseRowCo()
    {
        int nullCount = 0;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if(allDots[i, j] == null)
                {
                    nullCount++;
                }
                else if(nullCount > 0)
                {
                    allDots[i, j].GetComponent<Element>().row -= nullCount;
                    allDots[i, j] = null;
                }
            }
            nullCount = 0;
        }
        yield return new WaitForSeconds(refillDelay * 0.5f);
        StartCoroutine(FillBoardCo());
    }

    private void RefillBoard()
    {
        for( int i = 0; i < width; ++i )
        {
            for(int j = 0; j < height; ++j )
            {
                if (allDots[i, j] == null && !blankSpaces[i, j])
                {
                    Vector2 tempPosition = new Vector2(i, j + offSet);
                    int dotToUse = Random.Range(0, dots.Length);
                    int maxIterations = 0;
                    while(MatchesAt(i, j, dots[dotToUse]) && maxIterations < 100)
                    {
                        maxIterations++;
                        dotToUse = Random.Range(0, dots.Length);
                    }
                    maxIterations = 0;
                    GameObject dot = Instantiate(dots[dotToUse], tempPosition, Quaternion.identity);
                    allDots[i, j] = dot;
                    dot.GetComponent<Element>().row = j;
                    dot.GetComponent<Element>().column = i;
                }
            }
        }
    }

    private bool MatchesOnBoard()
    {
        findMatches.FindAllMatches();
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; ++j)
            {
                if (allDots[i, j] != null)
                {
                    if (allDots[i, j].GetComponent<Element>().isMatched)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private IEnumerator FillBoardCo()
    {
        RefillBoard();
        yield return new WaitForSeconds(refillDelay);
        while (MatchesOnBoard())
        {
            streakValue++;
            DestroyMatches();
            yield break;
        }
        currentElement = null;
        if(IsDeadlocked())
        {
            ShuffleBoard();
        }
        yield return new WaitForSeconds(refillDelay);
        if(currentState != GameState.pause)
        {
            currentState = GameState.move;
        }
        streakValue = 1;
    }

    private void SwitchPieces(int column, int row, Vector2 direction)
    {
        int newColumn = column + (int)direction.x;
        int newRow = row + (int)direction.y;
        if (newColumn >= 0 && newColumn < width && newRow >= 0 && newRow < height)
        {
            GameObject holder = allDots[newColumn, newRow] as GameObject;
            allDots[newColumn, newRow] = allDots[column, row];
            allDots[column, row] = holder;
        }
    }

    private bool CheckForMatches()
    {
        for(int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] != null)
                {
                    if (i < width - 2)
                    {
                        if (allDots[i + 1, j] != null && allDots[i + 2, j] != null)
                        {
                            if (allDots[i + 1, j].tag == allDots[i, j].tag
                                && allDots[i + 2, j].tag == allDots[i, j].tag)
                            {
                                return true;
                            }
                        }
                    }
                    if (j < height - 2)
                    {
                        if (allDots[i, j + 1] != null && allDots[i, j + 2] != null)
                        {
                            if (allDots[i, j + 1].tag == allDots[i, j].tag
                                && allDots[i, j + 2].tag == allDots[i, j].tag)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
        }
        return false;
    }

    public bool SwitchAndCheck(int column, int row, Vector2 direction)
    {
        SwitchPieces(column, row, direction);
        if(CheckForMatches())
        {
            SwitchPieces(column, row, direction);
            return true;
        }
        SwitchPieces(column, row, direction);
        return false;
    }

    private bool IsDeadlocked()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] != null)
                {
                    if (i < width - 1)
                    {
                        if (SwitchAndCheck(i, j, Vector2.right))
                        {
                            return false;
                        }
                    }
                    if (i < height - 1)
                    {
                        if (SwitchAndCheck(i, j, Vector2.up))
                        {
                            return false;
                        }
                    }
                }
            }
        }
        return true;
    }

    private void ShuffleBoard()
    {
        List<GameObject> newBoard = new List<GameObject>();
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] != null)
                {
                    newBoard.Add(allDots[i, j]);
                }
            }
        }
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (!blankSpaces[i, j])
                {
                    int pieceToUse = Random.Range(0, newBoard.Count);
                    int maxIterations = 0;
                    while (MatchesAt(i, j, newBoard[pieceToUse]) && maxIterations < 100)
                    {
                        pieceToUse = Random.Range(0, newBoard.Count);
                        maxIterations++;
                    }
                    Element piece = newBoard[pieceToUse].GetComponent<Element>();
                    maxIterations = 0;
                    piece.column = i;
                    piece.row = j;
                    allDots[i, j] = newBoard[pieceToUse];
                    newBoard.Remove(newBoard[pieceToUse]);
                }
            }
        }
        if(IsDeadlocked())
        {
            ShuffleBoard();
        }
    }
}
