using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Collections;
using DigitalRuby.Tween;

public class PuzzleGame : MonoBehaviour
{
    Texture2D texture;
    public GameObject piecePrefab;

    Sprite[] puzzleImage;
    GraphicRaycaster raycaster;

    PuzzlePiece selectedPiece, pieceToSwap;

    [HideInInspector]
    public int piecesLeftToSolve;

    bool isGameReady = false;

    private void Awake()
    {
        raycaster = GetComponent<GraphicRaycaster>();
    }

    public void SelectImageAndStart(Texture2D textureToSet)
    {
        texture = textureToSet;
        StartGame();
    }

    private void StartGame()
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
        if (gameObject.activeInHierarchy)
        {
            StartCoroutine("Shuffle"); 
        }
    }

    void Update()
    {
        if (!isGameReady) { return; }
        if (Input.GetMouseButtonDown(0))
        {
            CheckIfTouchingPiece();
        }
    }

    private void CheckIfPuzzleIfSolved()
    {
        if (piecesLeftToSolve == 0)
        {
            gameObject.SetActive(false);
        }
    }

    private void CheckIfTouchingPiece()
    {
        if (selectedPiece)
        {
            SelectPieceAndSwap();
        }
        else
        {
            SelectPieceIfSelectable();
        }
        CheckIfPuzzleIfSolved();
    }

    private void SelectPieceAndSwap()
    {
        pieceToSwap = RaycastForPiece();
        if (pieceToSwap == selectedPiece)
        {
            DeselectPieces();
        }
        else
        {
            SwapAndCheckPieces();
        }
    }

    private void SelectPieceIfSelectable()
    {
        if (RaycastForPiece().canBeSelected)
        {
            selectedPiece = RaycastForPiece();
            selectedPiece.MarkSelected();
        }
    }

    private void SwapAndCheckPieces()
    {
        if (selectedPiece && pieceToSwap.canBeSelected)
        {
            SwapPieces();
            CheckPieces();
            DeselectPieces();
        }
        else
        {
            DeselectPieces();
        }
    }

    private void SwapPieces()
    {
        int temp = selectedPiece.transform.GetSiblingIndex();
        selectedPiece.transform.SetSiblingIndex(pieceToSwap.transform.GetSiblingIndex());
        pieceToSwap.transform.SetSiblingIndex(temp);
    }
 
    private void CheckPieces()
    {
        selectedPiece.CheckIfInPosition();
        pieceToSwap.CheckIfInPosition();
    }

    private void DeselectPieces()
    {
        selectedPiece.UnmarkSelected();
        selectedPiece = null;
        pieceToSwap = null;
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

    IEnumerator Shuffle()
    {
        yield return new WaitForSeconds(2);
        foreach (Transform piece in transform)
        {
            int rNum = Random.Range(0, transform.childCount - 1);
            piece.SetSiblingIndex(rNum);
            yield return new WaitForSeconds(0.1f);
        }
        foreach (Transform piece in transform)
        {
            PuzzlePiece pPiece = piece.GetComponent<PuzzlePiece>();
            pPiece.currentPos = piece.transform.GetSiblingIndex();
            pPiece.CheckIfInPosition();
        }
        isGameReady = true;
    }

}
