using fNbt;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[RequireComponent(typeof(cakeslice.Outline))]
[RequireComponent(typeof(NetworkTransform))]
public class Piece : NetworkBehaviour {

    [SerializeField]
    private Text pieceValueText = null;
    [SerializeField]
    private Image pieceValueIcon = null;
    [SerializeField]
    private cakeslice.Outline outline = null;
    [SerializeField]
    private GameObject particlePrefab = null;

    [SyncVar]
    public int teamId;
    [SyncVar]
    public int pieceType;

    [ServerSideOnly]
    private MovementData moveData;

    public override void OnStartClient() {
        base.OnStartClient();

        // Color the piece to match it's team.
        this.GetComponent<MeshRenderer>().material = References.getMaterialFromTeam(Team.teamFromId(this.teamId));
    }

    private void Update() {
        // If this is on the Server, move the Piece towards it's destination.
        if(this.isServer) {
            if(this.moveData != null) {
                this.transform.position = moveData.getPos();

                if(this.moveData.isFinished()) {
                    this.moveData = null;
                }
            }
        }

        // Rotate the piece to face the camera.
        this.transform.rotation = Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0);

        // TODO can this be made better?
        if(this.isClient) {
            if(Player.localPlayer != null) {
                if(Player.localPlayer.isSpectating()) {
                    // Player is spectating, show all values.
                    this.showValue();
                }
                else {
                    // Only show the value if the piece belongs
                    // to the same team as the local player.
                    if(this.getTeam() == Player.localPlayer.getTeam()) {
                        this.showValue();
                    } else {
                        this.hideValue();
                    }
                }

                // Hide opponent piece if it's the startup
                bool hide = Player.localPlayer.board.gameState == GameStates.SETUP && Player.localPlayer.getTeam() != this.getTeam() && !Player.localPlayer.isSpectating();
                this.GetComponent<MeshRenderer>().enabled = !hide;
                this.GetComponentInChildren<Canvas>().enabled = !hide;
            }
        }
    }

    public NbtCompound writeToNbt() {
        NbtCompound tag = new NbtCompound();

        tag.setTag("pieceType", this.pieceType);
        tag.setTag("teamID", this.teamId);
        tag.setTag("position", this.transform.position);
        if(this.moveData != null) {
            tag.Add(this.moveData.writeToNbt());
        }

        return tag;
    }

    public void readFromNbt(NbtCompound tag) {
        this.pieceType = tag.getInt("pieceType");
        this.teamId = tag.getInt("teamID");
        this.transform.position = tag.getVector3("position");
        if(tag.Contains("moveData")) {
            this.moveData = new MovementData(tag.getCompound("moveData"));
        }
    }

    /// <summary>
    /// Reveals the value of the piece.
    /// </summary>
    public void showValue() {
        if(this.isNumberPiece()) {
            this.pieceValueText.text = this.getPieceLetter();
        }
        else {
            this.pieceValueIcon.enabled = true;
            this.pieceValueIcon.sprite = References.spriteFromPiece(this.pieceType);
        }
    }

    /// <summary>
    /// Hides the value of the piece.
    /// </summary>
    public void hideValue() {
        if(this.isNumberPiece()) {
            this.pieceValueText.text = string.Empty;
        } else {
            this.pieceValueIcon.enabled = false;
        }
    }

    /// <summary>
    /// Returns the possible Cells that this Piece can move to.
    /// An empty list is returned if it has no moves.
    /// </summary>
    public virtual List<Cell> getPossibleMoves() {
        List<Cell> possibleMoves = new List<Cell>();
        if(this.aloudToMove()) {
            foreach(Cell neighborCell in this.getCell().getNeighbors()) {
                Piece neighborPiece = neighborCell.getCurrentPiece();
                if(!neighborCell.isWater && (neighborPiece == null || neighborCell.getCurrentPiece().teamId != this.teamId)) {
                    possibleMoves.Add(neighborCell);
                }
            }
        }
        return possibleMoves;
    }

    /// <summary>
    /// Returns the attack outcome when this piece attacks the passed piece.
    /// </summary>
    public virtual EnumAttackOutcome getAttackOutcome(Piece otherPiece) {
        if(this.pieceType == PieceType.SPY) {
            if(otherPiece.pieceType == PieceType.BOMB || (!CustomNetworkManager.getSingleton().getGameOptions().spyKillAll.get() && otherPiece.pieceType != PieceType.ONE)) {
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
            throw new Exception("This piece (" + this.getPieceLetter() + ") should not be attacking!");
        }
    }

    /// <summary>
    /// Returns true if this piece is a numbered piece (1-9).
    /// </summary>
    public bool isNumberPiece() {
        return this.pieceType > 0;
    }

    /// <summary>
    /// Returns true if this piece can move.  False is returned for Bombs and Flags.
    /// </summary>
    public bool aloudToMove() {
        return this.pieceType != PieceType.BOMB && this.pieceType != PieceType.FLAG;
    }

    /// <summary>
    /// Returns the Team this piece belongs to.
    /// </summary>
    public Team getTeam() {
        return Team.teamFromId(this.teamId);
    }

    /// <summary>
    /// Sets the cell this Piece should move to.
    /// </summary>
    [ServerSideOnly]
    public void setDestination(Vector3 destination) {
        this.moveData = new MovementData(this.transform.position, destination);
    }

    /// <summary>
    /// Instantly sets the Piece's position.
    /// </summary>
    public void setPositionInstantly(Cell cell) {
        this.transform.position = cell.getPos();
    }

    /// <summary>
    /// Returns this Piece's letter.
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

    /// <summary>
    /// Enables or hides the outline effect.
    /// </summary>
    public void setOutlineVisible(bool isSelected) {
        this.outline.enabled = isSelected;
    }

    private void OnDestroy() {
        // Spawn a particle effect when the Piece is destroyed, only if the GameState is Playing.
        Board b = GameObject.FindObjectOfType<Board>();
        if(b != null && b.gameState == GameStates.PLAYING) {
            GameObject obj = GameObject.Instantiate(this.particlePrefab, this.transform.position, Quaternion.Euler(-90, 0, 0));
            ParticleSystem ps = obj.GetComponent<ParticleSystem>();
            ps.startColor = References.getMaterialFromTeam(this.getTeam()).color;
        }
    }
}