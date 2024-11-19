using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FindMatches : MonoBehaviour
{
    private Board board;
    public List<GameObject> currentMatches = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
    }

    public void FindAllMatches()
    {
        StartCoroutine(FindAllMatchesCo());
    }

    private IEnumerator FindAllMatchesCo()
    {
        yield return new WaitForSeconds(.2f);
        for (int i = 0; i < board.width; i++)
        {
            for (int j = 0; j < board.height; j++)
            {
                GameObject currentDot = board.allDots[i, j];
                if (currentDot != null)
                {
                    if (i > 0 && i < board.width - 1)
                    {
                        GameObject leftDot = board.allDots[i - 1, j];
                        GameObject rightDot = board.allDots[i + 1, j];
                        if (leftDot != null && rightDot != null)
                        {
                            if (leftDot.tag == currentDot.tag && rightDot.tag == currentDot.tag)
                            {
                                if(currentDot.GetComponent<Element>().isRowBomb ||
                                    leftDot.GetComponent<Element>().isRowBomb ||
                                    rightDot.GetComponent<Element>().isRowBomb)
                                {
                                    currentMatches.Union(GetRowPieces(j));
                                }
                                if(currentDot.GetComponent<Element>().isColumnBomb)
                                {
                                    currentMatches.Union(GetColumnPieces(i));
                                }
                                if (leftDot.GetComponent<Element>().isColumnBomb)
                                {
                                    currentMatches.Union(GetColumnPieces(i - 1));
                                }
                                if (rightDot.GetComponent<Element>().isColumnBomb)
                                {
                                    currentMatches.Union(GetColumnPieces(i + 1));
                                }
                                if (!currentMatches.Contains(leftDot))
                                {
                                    currentMatches.Add(leftDot);
                                }
                                leftDot.GetComponent<Element>().isMatched = true;
                                if (!currentMatches.Contains(rightDot))
                                {
                                    currentMatches.Add(rightDot);
                                }
                                rightDot.GetComponent<Element>().isMatched = true;
                                if (!currentMatches.Contains(currentDot))
                                {
                                    currentMatches.Add(currentDot);
                                }
                                currentDot.GetComponent<Element>().isMatched = true;
                            }
                        }
                    }
                    if (j > 0 && j < board.height - 1)
                    {
                        GameObject downDot = board.allDots[i, j - 1];
                        GameObject upDot = board.allDots[i, j + 1];
                        if (downDot != null && upDot != null)
                        {
                            if (upDot.tag == currentDot.tag && downDot.tag == currentDot.tag)
                            {
                                if (currentDot.GetComponent<Element>().isColumnBomb ||
                                    upDot.GetComponent<Element>().isColumnBomb ||
                                    downDot.GetComponent<Element>().isColumnBomb)
                                {
                                    currentMatches.Union(GetColumnPieces(i));
                                }
                                if (currentDot.GetComponent<Element>().isRowBomb)
                                {
                                    currentMatches.Union(GetRowPieces(j));
                                }
                                if (upDot.GetComponent<Element>().isRowBomb)
                                {
                                    currentMatches.Union(GetRowPieces(j + 1));
                                }
                                if (downDot.GetComponent<Element>().isRowBomb)
                                {
                                    currentMatches.Union(GetRowPieces(j - 1));
                                }
                                if (!currentMatches.Contains(downDot))
                                {
                                    currentMatches.Add(downDot);
                                }
                                downDot.GetComponent<Element>().isMatched = true;
                                if (!currentMatches.Contains(upDot))
                                {
                                    currentMatches.Add(upDot);
                                }
                                upDot.GetComponent<Element>().isMatched = true;
                                if (!currentMatches.Contains(currentDot))
                                {
                                    currentMatches.Add(currentDot);
                                }
                                currentDot.GetComponent<Element>().isMatched = true;
                            }
                        }
                    }
                }
            }
        }
    }

    public void GetPiecesOfColor(string color)
    {
        for (int i = 0; i < board.width; i++)
        {
            for(int j = 0; j < board.height; j++)
            {
                if (board.allDots[i, j] != null)
                {
                    if (board.allDots[i, j].tag == color)
                    {
                        board.allDots[i, j].GetComponent<Element>().isMatched = true;
                    }
                }
            }
        }
    }

    List<GameObject> GetColumnPieces(int column)
    {
        List<GameObject> elements = new List<GameObject>();
        for (int i = 0; i < board.height; i++)
        {
            if(board.allDots[column, i] != null)
            {
                elements.Add(board.allDots[column, i]);
                board.allDots[column, i].GetComponent<Element>().isMatched = true;
            }
        }
        return elements;
    }

    List<GameObject> GetRowPieces(int row)
    {
        List<GameObject> elements = new List<GameObject>();
        for (int i = 0; i < board.width; i++)
        {
            if (board.allDots[i, row] != null)
            {
                elements.Add(board.allDots[i, row]);
                board.allDots[i, row].GetComponent<Element>().isMatched = true;
            }
        }
        return elements;
    }

    public void CheckBombs()
    {
        if(board.currentElement != null)
        {
            if(board.currentElement.isMatched)
            {
                board.currentElement.isMatched = false;
                //int typeOfBomb = Random.Range(0, 100);
                //if (typeOfBomb < 50)
                //{
                //    board.currentElement.MakeRowBomb();
                //}
                //else if (typeOfBomb >= 50)
                //{
                //    board.currentElement.MakeColumnBomb();
                //}
                if((board.currentElement.swipeAngle > -45 && board.currentElement.swipeAngle <= 45)
                    || (board.currentElement.swipeAngle < -135 || board.currentElement.swipeAngle >= 135))
                {
                    board.currentElement.MakeRowBomb();
                }
                else
                {
                    board.currentElement.MakeColumnBomb();
                }
            }
            else if(board.currentElement.otherDot != null)
            {
                Element otherElement = board.currentElement.otherDot.GetComponent<Element>();
                //if(otherElement.isMatched)
                //{
                //    otherElement.isMatched = false;
                //    int typeOfBomb = Random.Range(0, 100);
                //    if (typeOfBomb < 50)
                //    {
                //        otherElement.MakeRowBomb();
                //    }
                //    else if (typeOfBomb >= 50)
                //    {
                //        otherElement.MakeColumnBomb();
                //    }
                //}
                if ((board.currentElement.swipeAngle > -45 && board.currentElement.swipeAngle <= 45)
                    || (board.currentElement.swipeAngle < -135 || board.currentElement.swipeAngle >= 135))
                {
                    otherElement.MakeRowBomb();
                }
                else
                {
                    otherElement.MakeColumnBomb();
                }
            }
        }
    }
}
