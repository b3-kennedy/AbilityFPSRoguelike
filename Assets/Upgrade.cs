using UnityEngine;

[CreateAssetMenu(fileName = "Upgrade", menuName = "Scriptable Objects/Upgrade")]
public class Upgrade : ScriptableObject
{
    public UpgradeEffect effect;
    public enum Rarity {COMMON, RARE, LEGENDARY};
    public Rarity rarity;
}
