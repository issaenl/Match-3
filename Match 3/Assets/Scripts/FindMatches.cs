using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Xml.Linq;
using Unity.VisualScripting;

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

    private List<GameObject> IsZoneBomb(Element element1, Element element2, Element element3)
    {
        List<GameObject> currentElements = new List<GameObject>();
        if (element1.isZoneBomb)
        {
            currentMatches.Union(GetZonePieces(element1.column, element1.row));
        }
        if (element2.isZoneBomb)
        {
            currentMatches.Union(GetZonePieces(element2.column, element2.row));
        }
        if (element3.isZoneBomb)
        {
            currentMatches.Union(GetZonePieces(element3.column, element3.row));
        }
        return currentElements;
    }

    private List<GameObject> IsRowBomb(Element element1, Element element2, Element element3)
    {
        List<GameObject> currentElements = new List<GameObject>();
        if (element1.isRowBomb)
        {
            currentMatches.Union(GetRowPieces(element1.row));
        }
        if (element2.isRowBomb)
        {
            currentMatches.Union(GetRowPieces(element2.row));
        }
        if (element3.isRowBomb)
        {
            currentMatches.Union(GetRowPieces(element3.row));
        }
        return currentElements;
    }

    private List<GameObject> IsColumnBomb(Element element1, Element element2, Element element3)
    {
        List<GameObject> currentElements = new List<GameObject>();
        if (element1.isColumnBomb)
        {
            currentMatches.Union(GetColumnPieces(element1.column));
        }
        if (element2.isColumnBomb)
        {
            currentMatches.Union(GetColumnPieces(element2.column));
        }
        if (element3.isColumnBomb)
        {
            currentMatches.Union(GetColumnPieces(element3.column));
        }
        return currentElements;
    }

    private void AddToListAndMatch(GameObject element)
    {
        if (!currentMatches.Contains(element))
        {
            currentMatches.Add(element);
        }
        element.GetComponent<Element>().isMatched = true;
    }

    private void GetNearbyElements(GameObject element1, GameObject element2, GameObject element3)
    {
        AddToListAndMatch(element1);
        AddToListAndMatch(element2);
        AddToListAndMatch(element3);
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
                    Element currentDotDot = currentDot.GetComponent<Element>();  
                    if (i > 0 && i < board.width - 1)
                    {
                        GameObject leftDot = board.allDots[i - 1, j];
                        GameObject rightDot = board.allDots[i + 1, j];
                        if (leftDot != null && rightDot != null)
                        {
                            Element leftDotDot = leftDot.GetComponent<Element>();
                            Element rightDotDot = rightDot.GetComponent<Element>();
                            if (leftDot.tag == currentDot.tag && rightDot.tag == currentDot.tag)
                            {
                                currentMatches.Union(IsRowBomb(leftDotDot, currentDotDot, rightDotDot));
                                currentMatches.Union(IsColumnBomb(leftDotDot, currentDotDot, rightDotDot));
                                currentMatches.Union(IsZoneBomb(leftDotDot, currentDotDot, rightDotDot));
                                GetNearbyElements(leftDot, currentDot, rightDot);
                            }
                        }
                    }
                    if (j > 0 && j < board.height - 1)
                    {
                        GameObject downDot = board.allDots[i, j - 1];
                        GameObject upDot = board.allDots[i, j + 1];
                        if (downDot != null && upDot != null)
                        {
                            Element downDotDot = downDot.GetComponent<Element>();
                            Element upDotDot = upDot.GetComponent<Element>();
                            if (upDot.tag == currentDot.tag && downDot.tag == currentDot.tag)
                            {
                                currentMatches.Union(IsColumnBomb(upDotDot, currentDotDot, downDotDot));
                                currentMatches.Union(IsRowBomb(upDotDot, currentDotDot, downDotDot));
                                currentMatches.Union(IsZoneBomb(upDotDot, currentDotDot, downDotDot));
                                GetNearbyElements(upDot, currentDot, downDot);
                            }
                        }
                    }
                }
            }
        }
    }

    List<GameObject> GetZonePieces(int column, int row)
    {
        List<GameObject> elements = new List<GameObject>();
        for (int i = column - 1; i <= column + 1; i++)
        {
            for (int j = row - 1; j <= row + 1; j++)
            {
                if(i >= 0 && i < board.width && j >= 0 && j < board.height)
                {
                    if (board.allDots[i, j] != null)
                    {
                        elements.Add(board.allDots[i, j]);
                        board.allDots[i, j].GetComponent<Element>().isMatched = true;
                    }
                }
            }
        }
        return elements;
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
                Element element = board.allDots[column, i].GetComponent<Element>();
                if (element.isRowBomb)
                {
                    elements.Union(GetRowPieces(i)).ToList();
                }
                if (element.isZoneBomb)
                {
                    elements.Union(GetZonePieces(column, i)).ToList();
                }
                elements.Add(board.allDots[column, i]);
                element.isMatched = true;
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
                Element element = board.allDots[i, row].GetComponent<Element>();
                if (element.isColumnBomb)
                {
                    elements.Union(GetColumnPieces(i)).ToList();
                }
                if (element.isZoneBomb)
                {
                    elements.Union(GetZonePieces(i, row)).ToList();
                }
                elements.Add(board.allDots[i, row]);
                element.isMatched = true;
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
