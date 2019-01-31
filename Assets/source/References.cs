using UnityEngine;

public class References : MonoBehaviour {

    public static References list;

    public void Awake() {
        References.list = this;
    }

    [Header("Pieces")]

    public GameObject pieceBomb;
    public GameObject pieceFlag;
    public GameObject pieceNumbered;
    public GameObject pieceSpy;

    [Header("Piece Materials")]

    public Material matPieceRed;
    public Material matPieceBlue;
    public Material matPieceYellow;
    public Material matPieceGreen;
    public Material matPieceOrange;
    public Material matPiecePurple;

    [Header("Base Ground Materials")]
    public Material groundRed;
    public Material groudBlue;
    public Material groundYellow;
    public Material groundGreen;
    public Material groundOrange;
    public Material groundPurple;

    [Header("Text")]
    public TextAsset tips;
}
