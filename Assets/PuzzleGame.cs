using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class PuzzleGame : MonoBehaviour
{
    public Texture2D texture;
    Sprite[] puzzleImage;
    public GameObject piecePrefab;

    GraphicRaycaster raycaster;

    PuzzlePiece selectedPiece;

    public int piecesLeftToSolve;

    private void Awake()
    {
        raycaster = GetComponent<GraphicRaycaster>();
    }

    void Start()
    {
        SetupPuzzle();
        ShufflePuzzle();
    }

    private void SetupPuzzle()
    {
        int pos = 0;
        puzzleImage = Resources.LoadAll<Sprite>(texture.name);
        foreach (Sprite puzzleImg in puzzleImage)
        {
            GameObject nPiece = Instantiate(piecePrefab, transform);
            nPiece.transform.SetSiblingIndex(pos);
            nPiece.GetComponent<PuzzlePiece>().pieceImage = puzzleImg;
            nPiece.GetComponent<PuzzlePiece>().startPos = pos;
            pos++;
        }
        piecesLeftToSolve = transform.childCount;
    }

    private void ShufflePuzzle()
    {

        foreach (Transform piece in transform)
        {
            int rNum = Random.Range(0, transform.childCount - 1);
            piece.SetSiblingIndex(rNum);       
        }

        foreach (Transform piece in transform)
        {
            piece.GetComponent<PuzzlePiece>().currentPos = piece.transform.GetSiblingIndex();
            piece.GetComponent<PuzzlePiece>().CheckIfInPosition();
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            CheckIfTouchingPiece();
        }
        if (piecesLeftToSolve == 0)
        {
            Destroy(gameObject);
        }
        Debug.Log("Solves left:" + piecesLeftToSolve);
    }

    private void CheckIfTouchingPiece()
    {

        if (selectedPiece)
        {
            PuzzlePiece pieceToSwap = RaycastForPiece();
            if (pieceToSwap == selectedPiece)
            {
                selectedPiece.UnmarkSelected();
                selectedPiece = null;
                return;
            }
            else
            {
                if (selectedPiece && pieceToSwap)
                {
                    SwapPieces(selectedPiece, pieceToSwap);
                    selectedPiece.UnmarkSelected();
                    selectedPiece = null;
                    pieceToSwap = null;
                }
            }
        }
        else
        {
            if (RaycastForPiece().canBeSelected)
            {
                selectedPiece = RaycastForPiece();
                selectedPiece.MarkSelected();
            }
        }
    }

    private void SwapPieces(PuzzlePiece a, PuzzlePiece b)
    {
        int temp = a.transform.GetSiblingIndex();
        a.transform.SetSiblingIndex(b.transform.GetSiblingIndex());
        b.transform.SetSiblingIndex(temp);
        a.CheckIfInPosition();
        b.CheckIfInPosition();
    }
 

    PuzzlePiece RaycastForPiece()
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        List<RaycastResult> results = new List<RaycastResult>();
        pointerData.position = Input.mousePosition;
        raycaster.Raycast(pointerData, results);

        GameObject selected = results[0].gameObject;
        PuzzlePiece puzzlePiece = selected.GetComponent<PuzzlePiece>();
        if (puzzlePiece != selectedPiece)
        {
            return puzzlePiece;
        }
        return selectedPiece;
    }
}
