using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[RequireComponent(typeof(cakeslice.Outline))]
[RequireComponent(typeof(NetworkTransform))]
public class Piece : NetworkBehaviour {

    [SerializeField] // For debug
    [SyncVar]
    public int teamId;
    private Text pieceName;
    private OutlineHelper outlineHelper;
    [SyncVar]
    public int pieceType;

    /// <summary> The position of the cell that the piece should be moving too. </summary>
    [ServerSideOnly]
    private Vector3? destination;

    private void Awake() {
        this.pieceName = this.GetComponentInChildren<Text>();
    }

    public override void OnStartClient() {
        this.outlineHelper = new OutlineHelper(this.gameObject);
        this.transform.rotation = Quaternion.Euler(0, Player.localPlayer.transform.eulerAngles.y, 0);

        this.showActualValue(false);

        this.GetComponent<MeshRenderer>().material.color = Team.teamFromId(this.teamId).getColor();
    }

    private void Update() {
        if(this.isServer) {
            if(this.destination != null) {
                Vector3 v = Vector3.Lerp(this.transform.position, (Vector3)this.destination, 0.1f);
                if(Vector3.Distance(v, (Vector3)this.destination) < 0.01f) {
                    this.transform.position = (Vector3)this.destination;
                    this.destination = null;

                    // The piece has stopped moving, cyle to the next turn.
                    GameObject.FindObjectOfType<Board>().nextTurn();
                } else {
                    this.transform.position = v;
                }
            }
        }
    }

    /// <summary>
    /// Sets the pieces text to show its true value.
    /// </summary>
    public void showActualValue(bool forceVisible) {
        if(this.teamId == Player.localPlayer.controllingTeamID || forceVisible) {
            this.pieceName.text = this.getPieceLetter();
        }
        else {
            this.pieceName.text = "?";
        }
    }

    /// <summary>
    /// Returns the possible Cells that this Piece can move to.
    /// </summary>
    public virtual List<Cell> getPossibleSquares() {
        List<Cell> list = new List<Cell>();
        foreach(Cell cell in this.getCell().neighbors) {
            if(!cell.isWater && cell.getCurrentPiece().teamId != this.teamId) {
                list.Add(cell);
            }
        }
        return list;
    }

    public virtual EnumAttackOutcome getAttackOutcome(Piece otherPiece) {
        if(this.pieceType == PieceType.SPY) {
            if(otherPiece.pieceType == PieceType.BOMB || (!GameOptions.spyKillAll && otherPiece.pieceType != PieceType.ONE)) {
                return EnumAttackOutcome.LOSE;
            }
            else {
                return EnumAttackOutcome.WIN;
            }
        } else if(this.isNumberPiece()) {
            if(otherPiece.pieceType == PieceType.BOMB) {
                return this.pieceType == 8 ? EnumAttackOutcome.WIN : EnumAttackOutcome.LOSE;
            }
            else if(otherPiece.isNumberPiece()) {
                int otherNumber = otherPiece.pieceType;
                if(this.pieceType == otherNumber) {
                    return EnumAttackOutcome.TIE;
                }
                else if(this.pieceType < otherNumber) {
                    return EnumAttackOutcome.WIN;
                }
                else { // this.number > otherNumber
                    return EnumAttackOutcome.LOSE;
                }
            }
            else { // Flag or spy.
                return EnumAttackOutcome.WIN;
            }
        } else {
            throw new Exception("A piece should either override this method, or return false for canMove()");
        }
    }

    /// <summary>
    /// Returns true if this piece is a numbered piece (1-9).
    /// </summary>
    public bool isNumberPiece() {
        return this.pieceType > 0;
    }

    /// <summary>
    /// Returns true if this piece can move.  False is returned for spies and flags.
    /// </summary>
    public bool canMove() {
        return this.pieceType != PieceType.BOMB && this.pieceType != PieceType.FLAG;
    }

    public void setDestination(Cell cell) {
        this.destination = cell.getPos();
    }

    /// <summary>
    /// Directly and instantly sets the Piece's position.
    /// </summary>
    public void setPosition(Cell cell) {
        this.transform.position = cell.getPos() + new Vector3(0, 0.1f, 0);
    }

    /// <summary>
    /// Returns the letter that this piece should show.
    /// </summary>
    public string getPieceLetter() {
        if(this.pieceType == PieceType.FLAG) {
            return "F";
        } else if(this.pieceType == PieceType.SPY) {
            return "S";
        } else if(this.pieceType == PieceType.BOMB) {
            return "B";
        } else if(this.isNumberPiece()) {
            return this.pieceType.ToString();
        }

        return "n";
    }

    /// <summary>
    /// Returns the Cell that this Piece is on.
    /// </summary>
    public Cell getCell() {
        RaycastHit hit;
        Physics.Raycast(this.transform.position + Vector3.up, Vector3.down, out hit, float.PositiveInfinity, Layers.CELL);
        return hit.transform.GetComponent<Cell>();
    }

    public void setSelected(bool flag) {
        this.outlineHelper.updateOutline(flag, EnumOutlineParam.SELECTED);
    }

    public static GameObject getPrefabFromType(int type) {
        switch(type) {
            case PieceType.SPY:
                return References.list.pieceSpy;
            case PieceType.BOMB:
                return References.list.pieceBomb;
            case PieceType.FLAG:
                return References.list.pieceFlag;
        }
        return References.list.pieceNumbered;
    }
}
