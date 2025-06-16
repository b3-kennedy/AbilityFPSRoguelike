using UnityEngine;

public class TargetedProjectile : MonoBehaviour
{

    protected Vector3 target;
    public virtual void SetTarget(Vector3 tgt)
    {
        target = tgt;
    }
}
