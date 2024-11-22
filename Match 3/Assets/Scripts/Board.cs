using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    wait,
    move
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


public class Board : MonoBehaviour
{
    public GameState currentState = GameState.move;
    public int width;
    public int height;
    public int offSet;
    public GameObject tilePrefab;
    public GameObject[] dots;
    public GameObject[,] allDots;
    public GameObject destroyEffect;
    public Element currentElement;
    public TileType[] boardLayout;
    private bool[,] blankSpaces;
    private FindMatches findMatches;
    
    
    // Start is called before the first frame update
    void Start()
    {
        findMatches = FindObjectOfType<FindMatches>();
        blankSpaces = new bool[width, height];
        allDots =  new GameObject[width, height];
        SetUp();
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

    private void SetUp()
    {
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

    private bool ColumnOrRow()
    {
        int numberHorizontal = 0;
        int numberVertical = 0;
        Element firstPiece = findMatches.currentMatches[0].GetComponent<Element>();
        if (firstPiece != null)
        {
            foreach (GameObject currentPiece in findMatches.currentMatches)
            {
                Element element = currentPiece.GetComponent<Element>();
                if (element.row == firstPiece.row)
                {
                    numberHorizontal++;
                }
                if (element.column == firstPiece.column)
                {
                    numberVertical++;
                }
            }
        }
        return (numberHorizontal == 5 || numberVertical == 5);
    }

    private void CheckToMakeBombs()
    {
        if(findMatches.currentMatches.Count == 4 || findMatches.currentMatches.Count == 7)
        {
            findMatches.CheckBombs();
        }
        if(findMatches.currentMatches.Count == 5 || findMatches.currentMatches.Count == 8)
        {
            if(ColumnOrRow())
            {
                if (currentElement != null)
                {
                    if (currentElement.isMatched)
                    {
                        if (!currentElement.isColorBomb)
                        {
                            currentElement.isMatched = false;
                            currentElement.MakeColorBomb();
                        }
                    }
                    else
                    {
                        if (currentElement.otherDot != null)
                        {
                            Element otherDot = currentElement.otherDot.GetComponent<Element>();
                            if (otherDot.isMatched)
                            {
                                if(!otherDot.isColorBomb)
                                {
                                    otherDot.isMatched = false;
                                    otherDot.MakeColorBomb();
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                if (currentElement != null)
                {
                    if (currentElement.isMatched)
                    {
                        if (!currentElement.isZoneBomb)
                        {
                            currentElement.isMatched = false;
                            currentElement.MakeZoneBomb();
                        }
                    }
                    else
                    {
                        if (currentElement.otherDot != null)
                        {
                            Element otherDot = currentElement.otherDot.GetComponent<Element>();
                            if (otherDot.isMatched)
                            {
                                if (!otherDot.isZoneBomb)
                                {
                                    otherDot.isMatched = false;
                                    otherDot.MakeZoneBomb();
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    private void DestroyMatchesAt(int column, int row)
    {
        if (allDots[column, row].GetComponent<Element>().isMatched)
        {
            if(findMatches.currentMatches.Count >= 4)
            {
                CheckToMakeBombs();
            }
            GameObject particle = Instantiate(destroyEffect, allDots[column, row].transform.position, Quaternion.identity);
            Destroy(particle, .5f);
            Destroy(allDots[column, row]);
            allDots[column, row] = null;
        }
    }

    public void DestroyMatches()
    {
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
        findMatches.currentMatches.Clear();
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
        yield return new WaitForSeconds(.5f);
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
        yield return new WaitForSeconds(.5f);
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
        yield return new WaitForSeconds(.6f);
        while(MatchesOnBoard())
        {
            yield return new WaitForSeconds(.6f);
            DestroyMatches();
        }
        findMatches.currentMatches.Clear();
        currentElement = null;
        yield return new WaitForSeconds(.6f);
        currentState = GameState.move;
    }
}
