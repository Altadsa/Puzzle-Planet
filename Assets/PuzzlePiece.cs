using UnityEngine;
using UnityEngine.UI;

public class PuzzlePiece : MonoBehaviour
{
    public Sprite pieceImage;

    public int startPos, currentPos;

    [HideInInspector]
    public bool isSeleted;

    [HideInInspector]
    public bool canBeSelected;

    private void Start()
    {
        GetComponent<Image>().sprite = pieceImage;
    }

    public void MarkSelected()
    {
        transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
        isSeleted = true;
    }

    public void UnmarkSelected()
    {
        transform.localScale = Vector3.one;
        isSeleted = false;
    }

    public void CheckIfInPosition()
    {
        currentPos = transform.GetSiblingIndex();
        Debug.Log(string.Format("Start Pos: {0} Current Pos: {1}", startPos, currentPos));
        if (currentPos == startPos)
        {
            canBeSelected = false;
            GetComponentInParent<PuzzleGame>().piecesLeftToSolve--;
        }
        else
        {
            canBeSelected = true;
        }
    }

}
