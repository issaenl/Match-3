using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraScalar : MonoBehaviour
{
    private Board board;
    public float cameraOffset = -10;
    public GameObject background;
    public float padding;
    /*public float rightOffset;
    private float cameraHeight;
    private float cameraWidth;*/

    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
        if (board != null)
        {
            RepositionCamera(board.width, board.height);
        }
    }

    void RepositionCamera(float x, float y)
    {
        /*Vector3 tempPosition = new Vector3(x / 2 - leftOffset, y / 2, cameraOffset);
        transform.position = tempPosition;
        if (board.width >= board.height)
        {
            Camera.main.orthographicSize = (board.width / 2 + padding) / aspectRatio;
        }
        else
        {
            Camera.main.orthographicSize = board.height / 2 + padding;
        }*/

        Vector3 cameraPosition = new Vector3(x / 2, y / 2, cameraOffset);
        transform.position = cameraPosition;
        float aspectRatio = (float)Screen.width / (float)Screen.height;
        float boardAspectRatio = x / y;
        if (aspectRatio >= boardAspectRatio)
        {
            Camera.main.orthographicSize = (y / 2) + padding;
            /*cameraHeight = Camera.main.orthographicSize * 2;
            cameraWidth = cameraHeight * aspectRatio;*/
        }
        else
        {
            Camera.main.orthographicSize = (x / 2 + padding) / aspectRatio;
            /*cameraWidth = Camera.main.orthographicSize * 2 * aspectRatio;
            cameraHeight = cameraWidth / aspectRatio;*/
        }

        /*GameObject back = Instantiate(background, transform.position, Quaternion.identity);
        Vector3 backgroundPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z + 1);
        back.transform.position = backgroundPosition;
        SpriteRenderer sr = back.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            float spriteWidth = sr.sprite.bounds.size.x;
            float spriteHeight = sr.sprite.bounds.size.y;
            float scaleX = cameraWidth / spriteWidth;
            float scaleY = cameraHeight / spriteHeight;
            back.transform.localScale = new Vector3(scaleX, scaleY, 1);
        }*/
    }
}
