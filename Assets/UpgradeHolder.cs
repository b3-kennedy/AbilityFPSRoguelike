using UnityEngine;

public class UpgradeHolder : Interactable
{
    public UpgradeEffect effect;

    public override void OnHover()
    {
        effect.GetToolTip().SetActive(true);
    }

    public override void OnUnHover()
    {
        effect.GetToolTip().SetActive(false);
    }
}
