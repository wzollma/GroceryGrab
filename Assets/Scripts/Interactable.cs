using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Interactable
{
    void onPickUp();
    void onDrop();
    void setHighlight(bool on);
    void move(Vector3 target, float moveSpeed);
    GameObject getGameObj();
    string getName();
    void disableNonGFXComponents();
    Vector3 getGFXCenterPos();
}
