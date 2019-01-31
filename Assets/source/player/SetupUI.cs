using System.Collections.Generic;
using UnityEngine;

public class SetupUI : MonoBehaviour {

    [SerializeField]
    private Canvas canvas;
    [SerializeField]
    private GameObject prefab;

    private SetupButton selectedPiece;
    private List<SetupButton> allButtons;

    public void createUI() {
        this.allButtons = new List<SetupButton>();

        this.func(PieceType.ONE, GameOptions.oneCount);
        this.func(PieceType.TWO, GameOptions.twoCount);
        this.func(PieceType.THREE, GameOptions.threeCount);
        this.func(PieceType.FOUR, GameOptions.fourCount);
        this.func(PieceType.FIVE, GameOptions.fiveCount);
        this.func(PieceType.SIX, GameOptions.sixCount);
        this.func(PieceType.SEVEN, GameOptions.sevenCount);
        this.func(PieceType.EIGHT, GameOptions.eightCount);
        this.func(PieceType.NINE, GameOptions.nineCount);
        this.func(PieceType.SPY, GameOptions.spyCount);
        this.func(PieceType.BOMB, GameOptions.bombCount);
        this.func(PieceType.FLAG, GameOptions.flagCount);
    }

    private const float SPACING = 40 + 15;
    private float xCount = SPACING;

    private void func(int pieceType, int count) {
        GameObject obj = GameObject.Instantiate(this.prefab, this.canvas.transform);
        obj.transform.position = new Vector3(this.xCount, SPACING);

        SetupButton btn = obj.GetComponent<SetupButton>();
        btn.set(this, pieceType, count);

        this.xCount += SPACING + 15;

        this.allButtons.Add(btn);
    }

    public SetupButton getSelectedPiece() {
        return this.selectedPiece;
    }

    public bool hasNoMorePieces() {
        bool noMore = true;
        foreach(SetupButton btn in this.allButtons) {
            noMore = btn.getCount() == 0;
        }
        return noMore;
    }

    public void setSelectedPiece(SetupButton btn) {
        if(this.selectedPiece != null) {
            this.selectedPiece.transform.localScale = Vector3.one;
        }

        this.selectedPiece = btn;
        if(this.selectedPiece != null) {
            const float f = 1.1f;
            this.selectedPiece.transform.localScale = new Vector3(f, f, f);
        }
    }

    public void increaseCount(int pieceType) {
        foreach(SetupButton btn in this.allButtons) {
            if(pieceType == btn.getPieceType()) {
                btn.increaseCount();
                break;
            }
        }
    }
}
