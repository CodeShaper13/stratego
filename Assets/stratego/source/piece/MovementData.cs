using fNbt;
using UnityEngine;

public class MovementData {

    private Vector3 startPos;
    private Vector3 endPos;
    private float timer;
    private float mul = 1f;

    public MovementData(Vector3 start, Vector3 end) {
        this.startPos = start;
        this.endPos = end;
        this.timer = Time.time;
    }

    public MovementData(NbtCompound tag) {
        this.startPos = tag.getVector3("startPos");
        this.endPos = tag.getVector3("endPos");
        this.timer = tag.getFloat("timer");
    }

    public Vector3 getPos() {
        return Vector3.Lerp(this.startPos, this.endPos, (Time.time - timer) * this.mul);
    }

    public bool isFinished() {
        return this.getPos() == this.endPos;
    }

    public NbtCompound writeToNbt() {
        NbtCompound tag = new NbtCompound("moveData");

        tag.setTag("startPos", this.startPos);
        tag.setTag("endPos", this.endPos);
        tag.setTag("timer", this.timer);

        return tag;
    }
}
