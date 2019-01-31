using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Player : NetworkBehaviour {

    public static Player localPlayer;

    // UI References:
    public Text announcementText;
    public Text cornerText;
    private float announcementTimer;
    public Toggle readyToggle;

    public SetupUI setupUI;

    [SyncVar]
    public bool isReady;

    public bool isSpectating;
    public bool isMyTurn;
    public bool movedThisTurn;

    [SyncVar]
    public int controllingTeamID;
    private Board board;

    public Camera playerCamera;
    private NetHandlerClient handler;

    /// <summary> The currently selected piece, may be null. </summary>
    [ClientSideOnly]
    private Piece selectedPiece;

    private void Start() {
        this.board = GameObject.FindObjectOfType<Board>();
    }

    public override void OnStartClient() {
        this.handler = new NetHandlerClient(this);
    }

    public override void OnStartLocalPlayer() {
        Player.localPlayer = this;

        this.playerCamera.gameObject.SetActive(true);
        this.setupUI.createUI();


        // DEBUG

        Cell cell;
        cell = GameObject.Find("Circle_014").GetComponent<Cell>();
        this.sendMessageToServer(new MessageAddPiece(cell.cellIndex, PieceType.SIX, 1));

        cell = GameObject.Find("Circle_174").GetComponent<Cell>();
        this.sendMessageToServer(new MessageAddPiece(cell.cellIndex, PieceType.NINE, 0));
    }

    private void Update() {
        if(!this.isLocalPlayer) {
            return;
        }

        if(Input.GetKeyDown(KeyCode.Escape)) {
            NetworkManager manager = NetworkManager.singleton;
            if(NetworkServer.active || manager.IsClientConnected()) {
                manager.StopHost();
                Main.singleton.state = 1;
            }
        }

        // Handle announcement text.
        if(this.announcementTimer > 0) {
            this.announcementTimer -= Time.deltaTime;
            if(this.announcementTimer <= 0) {
                this.announcementText.text = string.Empty;
            }
        }

        this.setupUI.gameObject.SetActive(this.board.gameState == 1);

        bool leftBtn = Input.GetMouseButtonDown(0);
        bool rightBtn = Input.GetMouseButtonDown(1);

        if(!this.isSpectating) {
            if(this.board.gameState == 1) {
                // Setup
                this.readyToggle.interactable = this.setupUI.hasNoMorePieces();

                RaycastHit hit;
                if(Physics.Raycast(this.playerCamera.ScreenPointToRay(Input.mousePosition), out hit, float.PositiveInfinity)) {
                    Cell hitCell = hit.transform.GetComponent<Cell>();
                    Piece hitPiece = hit.transform.GetComponent<Piece>();
                    if(leftBtn) {
                        // Remove a piece if we clicked one.
                        if(hitPiece != null && hitPiece.teamId == this.controllingTeamID) {
                            this.setupUI.increaseCount(hitPiece.pieceType);
                            // TODO piece dupe bug
                            //hitPiece.gameObject.SetActive(false); // Hide it, just in case the server is slow in receiving the message.
                            this.sendMessageToServer(new MessageRemovePiece(hitPiece.getCell().cellIndex));
                        }

                        // Add a piece if we clicked an empty square.
                        if(hitCell != null) {
                            if(hitCell.teamID == this.controllingTeamID && hitCell.getCurrentPiece() == null) {
                                SetupButton selectedPiece = this.setupUI.getSelectedPiece();
                                if(selectedPiece != null && selectedPiece.reduceCount()) {
                                    this.sendMessageToServer(new MessageAddPiece(hitCell.cellIndex, selectedPiece.getPieceType(), this.controllingTeamID));
                                }
                            }
                        }
                    }
                }
            } else if(this.board.gameState == 2) {
                // Playing
                if(this.isMyTurn && !this.movedThisTurn) {
                    // Handle input, it is this players turn.
                    if(leftBtn) {
                        RaycastHit hit;
                        if(Physics.Raycast(this.playerCamera.ScreenPointToRay(Input.mousePosition), out hit, float.PositiveInfinity)) {
                            Piece hitPiece = hit.transform.GetComponent<Piece>();
                            Cell hitCell = hit.transform.GetComponent<Cell>();

                            if(hitPiece != null) {
                                if(hitPiece.teamId == this.controllingTeamID && hitPiece.canMove()) {
                                    // Clicked a piece on our side
                                    if(this.selectedPiece == hitPiece) {
                                        // Deselect piece, we clicked the selected one..
                                        this.deSelSelected();
                                    }
                                    else {
                                        // Select the clicked piece.
                                        this.deSelSelected();
                                        this.selectedPiece = hitPiece;
                                        this.selectedPiece.setSelected(true);
                                    }
                                }
                                else {
                                    // Clicked an enemy piece.
                                    if(this.selectedPiece != null) {
                                        if(this.selectedPiece.getCell().isAdjacent(hitPiece.getCell())) {
                                            this.moveSelectedPiece(hitPiece.getCell());
                                        }
                                    }

                                }
                            }

                            if(hitCell != null && !hitCell.isWater && this.selectedPiece != null) {
                                if(hitCell.isAdjacent(this.selectedPiece.getCell())) {
                                    if(hitCell.getCurrentPiece() == null) {
                                        // move piece to it.
                                        this.moveSelectedPiece(hitCell);
                                    }
                                    else if(hitCell.getCurrentPiece().teamId != this.controllingTeamID) {
                                        // attack it.
                                        this.moveSelectedPiece(hitCell);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    private void moveSelectedPiece(Cell targetCell) {
        this.sendMessageToServer(new MessageMovePiece(this.selectedPiece.getCell(), targetCell));
        this.movedThisTurn = true;
        this.deSelSelected();
    }

    /// <summary>
    /// Shows the passed text to the player as an announcement.
    /// </summary>
    public void showText(string msg, float duration, bool showInCorner = false) {
        print(this.name + "Showing text: " + msg);
        if(showInCorner) {
            this.cornerText.text = msg;
        } else {
            this.announcementText.text = msg;
            this.announcementTimer = duration;
        }
    }

    /// <summary>
    /// Called whenever the ready toggle button is called. 
    /// </summary>
    public void callback_toggleClick() {
        this.isReady = this.readyToggle.isOn;
    }

    private void deSelSelected() {
        if(this.selectedPiece != null) {
            this.selectedPiece.setSelected(false);
            this.selectedPiece = null;
        }
    }

    /// <summary>
    /// Sends a message to the server.
    /// </summary>
    public void sendMessageToServer(AbstractMessageServer message) {
        base.connectionToServer.Send(message.getID(), message);
    }
}
