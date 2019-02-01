using System.Collections.Generic;
using UnityEngine;

public class SetupUI : MonoBehaviour {

    [SerializeField]
    private Canvas canvas;
    [SerializeField]
    private GameObject prefab;

    private SetupButton selectedPiece;
    public List<SetupButton> allButtons;

    public void createUI(Player p) {
        this.allButtons = new List<SetupButton>();

        this.func(p, PieceType.ONE, GameOptions.oneCount);
        this.func(p, PieceType.THREE, GameOptions.threeCount);
        this.func(p, PieceType.FOUR, GameOptions.fourCount);
        this.func(p, PieceType.FIVE, GameOptions.fiveCount);
        this.func(p, PieceType.SIX, GameOptions.sixCount);
        this.func(p, PieceType.SEVEN, GameOptions.sevenCount);
        this.func(p, PieceType.EIGHT, GameOptions.eightCount);
        this.func(p, PieceType.NINE, GameOptions.nineCount);
        this.func(p, PieceType.SPY, GameOptions.spyCount);
        this.func(p, PieceType.BOMB, GameOptions.bombCount);
        this.func(p, PieceType.FLAG, GameOptions.flagCount);
    }

    private const float SPACING = 40 + 15;
    private float xCount = SPACING;

    private void func(Player player, int pieceType, int maxCount) {
        GameObject obj = GameObject.Instantiate(this.prefab, this.canvas.transform);
        obj.transform.position = new Vector3(this.xCount, SPACING);

        SetupButton btn = obj.GetComponent<SetupButton>();
        btn.set(player, this, pieceType, maxCount);

        this.xCount += SPACING + 15;

        this.allButtons.Add(btn);
    }

    public SetupButton getSelectedPiece() {
        return this.selectedPiece;
    }

    public bool hasNoMorePieces() {
        bool noMore = true;
        foreach(SetupButton btn in this.allButtons) {
            noMore = btn.getPlacedCount() == 0;
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
}
