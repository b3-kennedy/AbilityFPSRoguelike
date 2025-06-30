using UnityEngine;

public class UnitData : MonoBehaviour
{

    public enum WeightClass {LIGHT, HEAVY};
    [SerializeField] WeightClass weightClass;
    public enum Team {GOOD, BAD, ABILITY};
    [SerializeField] Team team;

    public Team GetTeam() 
    {  
        return team; 
    }

    public WeightClass GetWeightClass()
    {
        return weightClass;
    }

}
