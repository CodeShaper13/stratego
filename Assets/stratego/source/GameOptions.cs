using fNbt;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Manages all of the options and piece counts for a game.
/// </summary>
public class GameOptions  {

    /// <summary> A list containing all of the options. </summary>
    public List<IOption> all = new List<IOption>();

    public OptionInt maxPlayers;
    public OptionInt turnTimeLimit;
    public OptionBool amphibiousFives;
    public OptionBool spyKillAll;
    public OptionBool nineLongMove;
    public OptionInt bombCount;
    public OptionInt oneCount;
    public OptionInt twoCount;
    public OptionInt threeCount;
    public OptionInt fourCount;
    public OptionInt fiveCount;
    public OptionInt sixCount;
    public OptionInt sevenCount;
    public OptionInt eightCount;
    public OptionInt nineCount;
    public OptionInt spyCount;

    public GameOptions(NbtCompound tag) : this() {
        foreach(IOption option in this.all) {
            option.readFromNbt(tag);
        }
    }

    public GameOptions() {
        this.maxPlayers = new OptionInt(this, "Player Count", "maxPlayers", 2, 1, 6);

        this.turnTimeLimit = new OptionInt(this, "Turn Time Limit", "turnTimeLimit", 15);
        this.amphibiousFives = new OptionBool(this, "Amphibious Fives", "amphibiousFives", false);
        this.spyKillAll = new OptionBool(this, "Spy Defeat All", "spyKillAll", true);
        this.nineLongMove = new OptionBool(this, "Nine Long Move", "nineLongMove", false);

        this.bombCount = new OptionInt(this, "Bomb Count", "pCountB", 6, 0, 10);
        this.oneCount = new OptionInt(this, "One Count", "pCount1", 1, 0, 10);
        this.twoCount = new OptionInt(this, "Two Count", "pCount2", 1, 0, 10);
        this.threeCount = new OptionInt(this, "Three Count", "pCount3", 2, 0, 10);
        this.fourCount = new OptionInt(this, "Four Count", "pCount4", 3, 0, 10);
        this.fiveCount = new OptionInt(this, "Five Count", "pCount5", 3, 0, 10); // 4;
        this.sixCount = new OptionInt(this, "Six Count", "pCount6", 3, 0, 10); // 4;
        this.sevenCount = new OptionInt(this, "Seven Count", "pCount7", 4, 0, 10);
        this.eightCount = new OptionInt(this, "Eight Count", "pCount8", 5, 0, 10);
        this.nineCount = new OptionInt(this, "Nine Count", "pCount9", 6, 0, 10); // 8;
        this.spyCount = new OptionInt(this, "Spy Count", "pCountS", 1, 0, 10);
    }

    public GameOptions(NetworkReader reader) : this() {
        foreach(IOption option in this.all) {
            option.deserialize(reader.ReadInt32());
        }
    }

    public void serializeGameOptions(NetworkWriter writer) {
        foreach(IOption option in this.all) {
            writer.Write(option.getSerializeValue());
        }
    }

    public NbtCompound writeToNbt(NbtCompound tag) {
        foreach(IOption option in this.all) {
            option.saveToNbt(tag);
        }
        return tag;
    }

    public interface IOption {
        string getPropName();

        string getDisplayName();

        string prettyPrint();

        int getSerializeValue();

        void deserialize(int value);

        void saveToNbt(NbtCompound tag);

        void readFromNbt(NbtCompound tag);
    }

    public abstract class Option<T> : IOption {

        protected string displayName;
        protected string propName;
        protected string infoTransKey;

        public Option(GameOptions options, string displayName, string propName, T defaultValue, string infoTransKey = "") {
            options.all.Add(this);

            this.displayName = displayName;
            this.propName = propName;
            this.infoTransKey = infoTransKey;
            this.set(defaultValue);
        }

        public string getPropName() {
            return this.propName;
        }

        public string getDisplayName() {
            return this.displayName;
        }

        public abstract string prettyPrint();

        public abstract T get();

        public abstract int getSerializeValue();

        public abstract void deserialize(int value);

        public abstract void set(T value);

        public abstract void saveToNbt(NbtCompound tag);

        public abstract void readFromNbt(NbtCompound tag);
    }

    public class OptionBool : Option<bool> {

        public OptionBool(GameOptions options, string s1, string s, bool flag) : base(options, s1, s, flag) { }

        private bool flag;

        public override string prettyPrint() {
            string c = ColorUtility.ToHtmlStringRGBA(this.get() ? UiColors.GREEN : UiColors.RED);
            return this.getDisplayName() + ": " + "<color=#" + c + ">" + this.get() + "</color>";
        }

        public override bool get() {
            return this.flag;
        }

        public override int getSerializeValue() {
            return this.get() ? 1 : 0;
        }

        public override void deserialize(int value) {
            this.flag = value == 1;
        }

        public override void readFromNbt(NbtCompound tag) {
            this.flag = tag.getBool(this.propName);
        }

        public override void saveToNbt(NbtCompound tag) {
            tag.setTag(this.propName, this.flag);
        }

        public override void set(bool value) {
            this.flag = value;
        }
    }

    public class OptionInt : Option<int> {

        private readonly int minValue;
        private readonly int maxValue;
        private int value;

        public OptionInt(GameOptions options, string displayName, string propName, int defaultValue, int minValue = 0, int maxValue = 10) : base(options, displayName, propName, defaultValue) {
            this.minValue = minValue;
            this.maxValue = maxValue;
        }

        public int getMinValue() {
            return this.minValue;
        }

        public int getMaxValue() {
            return this.maxValue;
        }

        public override string prettyPrint() {
            string c = ColorUtility.ToHtmlStringRGBA(UiColors.YELLOW);
            return this.getDisplayName() + ": " + "<color=#" + c + ">" + this.get() + "</color>";
        }

        public override int get() {
            return this.value;
        }

        public override int getSerializeValue() {
            return this.get();
        }

        public override void deserialize(int value) {
            this.value = value;
        }

        public override void readFromNbt(NbtCompound tag) {
            this.value = tag.getInt(this.propName);
        }

        public override void saveToNbt(NbtCompound tag) {
            tag.setTag(this.propName, this.value);
        }

        public override void set(int value) {
            this.value = value;
        }
    }
}
