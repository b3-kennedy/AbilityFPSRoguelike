using UnityEngine;

public class UnitData : MonoBehaviour
{
    public enum Team {GOOD, BAD};
    [SerializeField] Team team;

    public Team GetTeam() 
    {  
        return team; 
    }

}
