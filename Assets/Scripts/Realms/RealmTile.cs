using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RealmTile : MonoBehaviour
{
    public Realm realm;
    public Color realmColor;
}

public enum Realm
{
    Default, Water, Fire, Earth
}
