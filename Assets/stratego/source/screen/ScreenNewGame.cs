using UnityEngine;
using UnityEngine.UI;

public class ScreenNewGame : ScreenBase {

    [SerializeField]
    private Transform holder;
    [SerializeField]
    private Text textPieceCount;
    [SerializeField]
    private Button btnStartGame;

    private GameOptions options;
    private Slider[] pcs;

    private bool initializing;

    public override void onUiOpen() {
        this.initializing = true;

        this.options = new GameOptions();
        this.pcs = new Slider[11];

        // Generate fields for all of the game options
        x = 500;
        y = DEFAULT_Y;
        this.pcs[0] = this.addSlider(this.options.oneCount);
        this.pcs[1] = this.addSlider(this.options.twoCount);
        this.pcs[2] = this.addSlider(this.options.threeCount);
        this.pcs[3] = this.addSlider(this.options.fourCount);
        this.pcs[4] = this.addSlider(this.options.fiveCount);
        this.pcs[5] = this.addSlider(this.options.sixCount);
        this.pcs[6] = this.addSlider(this.options.sevenCount);
        this.pcs[7] = this.addSlider(this.options.eightCount);
        this.pcs[8] = this.addSlider(this.options.nineCount);
        this.pcs[9] = this.addSlider(this.options.bombCount);
        this.pcs[10] = this.addSlider(this.options.spyCount);

        x = 150;
        y = DEFAULT_Y;
        this.addSlider(this.options.maxPlayers);
        this.addToggle(this.options.amphibiousFives);
        this.addToggle(this.options.spyKillAll);
        this.addToggle(this.options.nineLongMove);

        this.resetPieceCountSliders(-1, this.options.maxPlayers.get());
        this.updatePieceCountText();

        this.initializing = false;
    }

    public override bool showPhotographBackground() {
        return true;
    }

    public override void onEscape() {
        this.screenManager.showScreen(this.screenManager.screenMainMenu);
    }

    public override void onUiClose() {
        foreach(Transform t in this.holder) {
            GameObject.Destroy(t.gameObject);
        }
    }

    // Fields for the addSlider() and addToggle() methods.
    private float x = 200;
    private float y = DEFAULT_Y;
    private const float DEFAULT_Y = -30;

    private Slider addSlider(GameOptions.OptionInt option) {
        Slider slide = this.func(References.list.sliderPrefab).GetComponent<Slider>();
        slide.minValue = option.getMinValue();
        slide.maxValue = option.getMaxValue();
        slide.value = option.get();

        slide.onValueChanged.AddListener(delegate {
            callback_onSliderValueChange(slide, option);
            func();
        });

        this.updateSliderText(slide, option);

        return slide;
    }

    private Toggle addToggle(GameOptions.OptionBool option) {
        Toggle tog = this.func(References.list.togglePrefab).GetComponent<Toggle>();
        tog.isOn = option.get();
        tog.onValueChanged.AddListener(delegate {
            callback_onToggleClick(tog, option);
            func();
        });

        // Set the label text
        Text t = tog.GetComponentInChildren<Text>();
        t.text = option.getDisplayName();

        return tog;
    }

    private GameObject func(GameObject prefab) {
        GameObject obj = GameObject.Instantiate(prefab, this.holder);
        obj.transform.localPosition = new Vector3(x, y);
        y -= 30;
        return obj;
    }

    private void func() {
        if(!this.initializing) {
            this.playBtnSound();
        }
    }

    public void callback_onToggleClick(Toggle tog, GameOptions.OptionBool option) {
        option.set(tog.isOn);
    }

    public void callback_onSliderValueChange(Slider slider, GameOptions.OptionInt option) {
        bool isPlayerCountSlider = option.getPropName() == "maxPlayers";
        bool isPieceCountSlider = option.getPropName().StartsWith("pCount");

        int oldVlaue = option.get();
        int newValue = (int)slider.value;

        // Hacky
        if(isPlayerCountSlider && newValue == 1) {
            newValue = 2;
            slider.value = 2;
        }

        option.set(newValue);
        this.updateSliderText(slider, option);

        // If this slider is for a piece count
        if(isPieceCountSlider) {
            this.updatePieceCountText();
        }

        if(isPlayerCountSlider) {
            this.resetPieceCountSliders(oldVlaue, newValue);
        }
    }

    public void callback_startGame() {
        this.screenManager.showScreen(null);
        this.getNetworkManager().startGameServer(this.options, null);
    }

    private void resetPieceCountSliders(int oldVlaue, int newValue) {
        BoardType oldBoard = BoardType.getBoardFromPlayerCount(oldVlaue);
        BoardType nB = BoardType.getBoardFromPlayerCount(newValue);
        if(oldBoard.getMaxPieces() != nB.getMaxPieces()) {
            // Boards have different max pieces, set default piece amounts.
            this.pcs[0].value = nB.oneCount;
            this.pcs[1].value = nB.twoCount;
            this.pcs[2].value = nB.threeCount;
            this.pcs[3].value = nB.fourCount;
            this.pcs[4].value = nB.fiveCount;
            this.pcs[5].value = nB.sixCount;
            this.pcs[6].value = nB.sevenCount;
            this.pcs[7].value = nB.eightCount;
            this.pcs[8].value = nB.nineCount;
            this.pcs[9].value = nB.bombCount;
            this.pcs[10].value = nB.spyCount;
        }
    }

    private void updateSliderText(Slider slider, GameOptions.OptionInt option) {
        slider.GetComponentInChildren<Text>().text = option.getDisplayName() + ": " + slider.value;
    }

    private void updatePieceCountText() {
        int pieceCount = 1; // The flag.
        foreach(Slider s in this.pcs) {
            pieceCount += (int)s.value;
        }
        int requiredPieces = BoardType.getBoardFromPlayerCount(this.options.maxPlayers.get()).getMaxPieces();

        bool flag = pieceCount == requiredPieces;
        string c = ColorUtility.ToHtmlStringRGBA(flag ? UiColors.GREEN : UiColors.RED);
        string text = "<color=#" + c + ">" + pieceCount + "/" + requiredPieces + "</color>";

        this.textPieceCount.text = text;
        this.btnStartGame.interactable = flag;
    }
}
