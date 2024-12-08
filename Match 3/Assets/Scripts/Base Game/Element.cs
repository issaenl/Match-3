using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Element : MonoBehaviour
{
    public int column;
    public int row;
    public int previousColumn;
    public int previousRow;
    public int targetX;
    public int targetY;
    public bool isMatched = false;
    public float swipeAngle = 0;
    public float swipeResist = 1f;
    public GameObject otherDot;
    public bool isColumnBomb;
    public bool isRowBomb;
    public GameObject rowArrow;
    public GameObject columnArrow;
    public bool isColorBomb;
    public GameObject colorBomb;
    public bool isZoneBomb;
    public GameObject zoneBomb;
    private HintManager hintManager;
    private FindMatches findMatches;
    private GameManager gameManager;
    private Board board;
    private Vector2 firstTouchPosition = Vector2.zero;
    private Vector2 finalTouchPosition = Vector2.zero;
    private Vector2 tempPosition;


    // Start is called before the first frame update
    void Start()
    {
        isColumnBomb = false;
        isRowBomb = false;
        isColorBomb = false;
        isZoneBomb = false;
        hintManager = FindObjectOfType<HintManager>();
        gameManager = FindObjectOfType<GameManager>();
        board = GameObject.FindWithTag("Board").GetComponent<Board>();
        //board = FindObjectOfType<Board>();
        findMatches = FindObjectOfType<FindMatches>();
    }


    // Update is called once per frame
    void Update()
    {
        SpriteRenderer mySprite = GetComponent<SpriteRenderer>();
        if (isMatched)
        {
            mySprite.color = Color.grey;
        }
        else
        {
            mySprite.color = Color.white;
        }
        targetX = column;
        targetY = row;
        if (Mathf.Abs(targetX - transform.position.x) > .1)
        {
            tempPosition = new Vector2(targetX, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, tempPosition, .4f);
            if (board.allDots[column, row] != this.gameObject)
            {
                board.allDots[column, row] = this.gameObject;
                findMatches.FindAllMatches();
            }
        }
        else
        {
            tempPosition = new Vector2(targetX, transform.position.y);
            transform.position = tempPosition;
        }
        if (Mathf.Abs(targetY - transform.position.y) > .1)
        {
            tempPosition = new Vector2(transform.position.x, targetY);
            transform.position = Vector2.Lerp(transform.position, tempPosition, .4f);
            if (board.allDots[column, row] != this.gameObject)
            {
                board.allDots[column, row] = this.gameObject;
                findMatches.FindAllMatches();
            }
        }
        else
        {
            tempPosition = new Vector2(transform.position.x, targetY);
            transform.position = tempPosition;
        }
    }

    public IEnumerator CheckMoveCo()
    {
        if(isColorBomb)
        {
            findMatches.GetPiecesOfColor(otherDot.tag);
            isMatched = true;
        }
        else if(otherDot.GetComponent<Element>().isColorBomb)
        {
            findMatches.GetPiecesOfColor(this.gameObject.tag);
            otherDot.GetComponent<Element>().isMatched = true;
        }
        yield return new WaitForSeconds(.3f);
        if (otherDot != null)
        {
            if (!isMatched && !otherDot.GetComponent<Element>().isMatched)
            {
                int tempRow = this.row;
                int tempCol = this.column;
                this.row = otherDot.GetComponent<Element>().row;
                this.column = otherDot.GetComponent<Element>().column;
                otherDot.GetComponent<Element>().row = tempRow;
                otherDot.GetComponent<Element>().column = tempCol;
                yield return new WaitForSeconds(.5f);
                board.currentElement = null;
            }
            else
            {
                if(gameManager != null)
                {
                    if(gameManager.requirements.gameType == GameType.Moves)
                    {
                        gameManager.DecreaseCounterValue();
                    }
                }
                board.DestroyMatches();
            }
            otherDot = null;
            board.currentState = GameState.move;
        }
    }

    void OnMouseDown()
    {
        if(hintManager != null)
        {
            hintManager.DestroyHint();
        }
        if(board.currentState == GameState.move)
        {
            firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            GetComponent<SpriteRenderer>().color = Color.grey;
        }
    }

    private void OnMouseUp()
    {
        if (board.currentState == GameState.move)
        {
            finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            GetComponent<SpriteRenderer>().color = Color.white;
            CalculateAngle();
        }
    }

    void CalculateAngle()
    {
        if(Mathf.Abs(finalTouchPosition.y - firstTouchPosition.y) > swipeResist
           || Mathf.Abs(finalTouchPosition.x - firstTouchPosition.x) > swipeResist)
        {
            board.currentState = GameState.wait;
            swipeAngle = Mathf.Atan2(finalTouchPosition.y - firstTouchPosition.y, finalTouchPosition.x - firstTouchPosition.x) * 180 / Mathf.PI;
            MovePieces();
            board.currentElement = this;
        }
        else
        {
            board.currentState = GameState.move;
        }
    }

    void MovePiecesActual(Vector2 direction)
    {
        otherDot = board.allDots[column + (int)direction.x, row + (int)direction.y];
        previousRow = row;
        previousColumn = column;
        if (otherDot != null)
        {
            otherDot.GetComponent<Element>().column += -1 * (int)direction.x;
            otherDot.GetComponent<Element>().row += -1 * (int)direction.y;
            column += (int)direction.x;
            row += (int)direction.y;
            StartCoroutine(CheckMoveCo());
        }
        else
        {
            board.currentState = GameState.move;
        }
    }

    void MovePieces()
    {
        if (swipeAngle > -45 && swipeAngle <= 45 && column < board.width - 1)
        {
            //rigth swipe
            MovePiecesActual(Vector2.right);
        }
        else if (swipeAngle > 45 && swipeAngle <= 135 && row < board.height - 1) 
        {
            //up swipe
            MovePiecesActual(Vector2.up);
        }
        else if ((swipeAngle > 135 || swipeAngle <= -135) && column > 0)
        {
            //left swipe
            MovePiecesActual(Vector2.left);
        }
        else if (swipeAngle < -45 && swipeAngle >= -135 && row > 0)
        {
            //down swipe
            MovePiecesActual(Vector2.down);
        }
        else
        {
            board.currentState = GameState.move;
        }
    }

    void FindMatches()
    {
        if (column > 0 && column < board.width - 1)
        {
            GameObject leftDot1 = board.allDots[column - 1, row];
            GameObject rightDot1 = board.allDots[column + 1, row];
            if(leftDot1 != null && rightDot1 != null)
            {
                if (leftDot1.tag == this.gameObject.tag && rightDot1.tag == this.gameObject.tag)
                {
                    leftDot1.GetComponent<Element>().isMatched = true;
                    rightDot1.GetComponent<Element>().isMatched = true;
                    isMatched = true;
                }
            }
        }
        if (row > 0 && row < board.height - 1)
        {
            GameObject upDot1 = board.allDots[column, row + 1];
            GameObject downDot1 = board.allDots[column, row - 1];
            if (upDot1 != null && downDot1 != null)
            {
                if (upDot1.tag == this.gameObject.tag && downDot1.tag == this.gameObject.tag)
                {
                    upDot1.GetComponent<Element>().isMatched = true;
                    downDot1.GetComponent<Element>().isMatched = true;
                    isMatched = true;
                }
            }
        }
    }

    public void MakeRowBomb()
    {
        if (!isColumnBomb && !isColorBomb && !isZoneBomb)
        {
            isRowBomb = true;
            GameObject arrow = Instantiate(rowArrow, transform.position, Quaternion.identity);
            arrow.transform.parent = this.transform;
        }
    }

    public void MakeColumnBomb()
    {
        if (!isRowBomb && !isColorBomb && !isZoneBomb)
        {
            isColumnBomb = true;
            GameObject arrow = Instantiate(columnArrow, transform.position, Quaternion.identity);
            arrow.transform.parent = this.transform;
        }
    }

    public void MakeColorBomb()
    {
        if (!isRowBomb && !isColumnBomb && !isZoneBomb)
        {
            isColorBomb = true;
            GameObject color = Instantiate(colorBomb, transform.position, Quaternion.identity);
            color.transform.parent = this.transform;
            this.gameObject.tag = "Color";
        }
    }

    public void MakeZoneBomb()
    {
        if (!isRowBomb && !isColumnBomb && !isColorBomb)
        {
            isZoneBomb = true;
            GameObject zone = Instantiate(zoneBomb, transform.position, Quaternion.identity);
            zone.transform.parent = this.transform;
        }
    }
}
