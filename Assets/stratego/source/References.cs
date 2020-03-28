using UnityEngine;

public class References : MonoBehaviour {

    public static References list;

    public void Awake() {
        References.list = this;
    }

    [Header("Piece")]
    public GameObject gamePiece;

    [Header("Piece Materials")]
    public Material matPieceRed;
    public Material matPieceBlue;
    public Material matPieceYellow;
    public Material matPieceGreen;
    public Material matPieceOrange;
    public Material matPiecePurple;

    [Header("Ground Materials")]
    public Material groundGrass;
    public Material groundWater;

    [Header("Piece Icons")]
    public Sprite pieceIconBomb;
    public Sprite pieceIconFlag;
    public Sprite pieceIconSpy;

    [Header("Text")]
    public TextAsset tips;
    public TextAsset credits;

    [Header("Board")]
    public BoardType[] boards;

    [Header("Generated UI Elements")]
    public GameObject sliderPrefab;
    public GameObject togglePrefab;

    public static Material getMaterialFromTeam(Team team) {
        if(team == Team.RED) {
            return References.list.matPieceRed;
        }
        else if(team == Team.BLUE) {
            return References.list.matPieceBlue;
        }
        else if(team == Team.YELLOW) {
            return References.list.matPieceYellow;
        }
        else if(team == Team.GREEN) {
            return References.list.matPieceGreen;
        }
        else if(team == Team.PURPLE) {
            return References.list.matPiecePurple;
        }
        else if(team == Team.ORANGE) {
            return References.list.matPieceOrange;
        }
        return null;
    }

    public static Sprite spriteFromPiece(int pieceType) {
        if(pieceType == PieceType.BOMB) {
            return References.list.pieceIconBomb;
        }
        else if(pieceType == PieceType.FLAG) {
            return References.list.pieceIconFlag;
        }
        else {
            return References.list.pieceIconSpy;
        }
    }
}