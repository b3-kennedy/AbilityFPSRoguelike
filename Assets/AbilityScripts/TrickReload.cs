using UnityEngine;

public class TrickReload : Ability
{

    Gun gun;

    Transform reloadInterface;
    Transform reloadMarker;
    Transform reloadPoint;

    RectTransform reloadInterfaceRect;

    Vector2 endPoint;
    Vector2 startPoint;

    float timer = 0;
    public float duration = 1f;

    float pointStart;
    float pointEnd;

    bool hasMissed = false;

    public float buffer;
    public float reloadPointWidth;
    public float damageIncrease = 0.5f;

    int combo;

    public override void OnInitialise()
    {
        base.OnInitialise();
        gun = GetCaster().GetComponent<PlayerData>().GetGunParent().GetChild(0).GetChild(0).GetComponent<Gun>();
        timer = 0;
        reloadInterface = GetCaster().GetComponent<PlayerInterfaceManager>().playerInterface.transform.GetChild(4);
        reloadMarker = reloadInterface.GetChild(1);
        reloadPoint =   reloadInterface.GetChild(0);
        reloadInterface.gameObject.SetActive(false);
        reloadInterfaceRect = reloadInterface.GetComponent<RectTransform>();
        hasMissed = false;
        combo = 0;
        gun.reload.RemoveAllListeners();
        gun.reload.AddListener(Reload);

        gun.shotHit.RemoveAllListeners();
        gun.shotHit.AddListener(Hit);


    }

    void Hit()
    {
        gun.SetDamageMultiplier(combo * damageIncrease);
    }

    void Reload()
    {
        hasMissed = false;

    }

    public override void UpdateAbility()
    {
        if (gun.IsReloading())
        {
            if (!reloadInterface.gameObject.activeSelf && !hasMissed)
            {
                timer = 0;
                float interfaceWidth = reloadInterfaceRect.rect.width;
                endPoint = new Vector2(reloadInterface.transform.position.x + interfaceWidth/2, reloadInterface.transform.position.y);
                startPoint = new Vector2(reloadInterface.position.x - interfaceWidth / 2, reloadInterface.transform.position.y);
                reloadInterface.gameObject.SetActive(true);
                reloadMarker.GetComponent<RectTransform>().position = startPoint;
                float pointPosition = Random.Range(startPoint.x + buffer + reloadPoint.GetComponent<RectTransform>().rect.width, 
                    endPoint.x - buffer - reloadPoint.GetComponent<RectTransform>().rect.width);
                reloadPoint.transform.position = new Vector2(pointPosition, reloadInterface.position.y);
                float widthMultiplier = reloadPoint.GetComponent<RectTransform>().rect.width / 2 * reloadPoint.transform.localScale.x;
                pointStart = reloadPoint.transform.position.x - widthMultiplier;
                pointEnd = reloadPoint.transform.position.x + widthMultiplier;
                reloadPoint.transform.localScale = new Vector2(reloadPointWidth, reloadPoint.transform.localScale.y);

                Debug.Log(combo);
            }

            timer += Time.deltaTime;
            float t = timer / duration;
            t = Mathf.Clamp01(t);
            reloadMarker.GetComponent<RectTransform>().position = Vector2.Lerp(startPoint, endPoint, t);

            RectTransform markerRect = reloadMarker.GetComponent<RectTransform>();
            float markerHalfWidth = markerRect.rect.width / 2f;
            float markerLeft = reloadMarker.transform.position.x - markerHalfWidth;
            float markerRight = reloadMarker.transform.position.x + markerHalfWidth;

            // Check for any overlap between the marker and the reload zone
            bool isOverlapping = markerRight >= pointStart && markerLeft <= pointEnd;

            if (Input.GetButtonDown("Fire2") || Input.GetKeyDown(KeyCode.R) && !hasMissed)
            {
                if (isOverlapping)
                {
                    gun.FinishReload();
                    combo++;
                    
                }
                else
                {
                    hasMissed = true;
                    combo = 0;
                }
                
            }

            if (reloadMarker.transform.position.x >= endPoint.x)
            {
                combo = 0;
                hasMissed = true;
                reloadInterface.gameObject.SetActive(false);
            }


        }
        else if(!gun.IsReloading() && reloadInterface.gameObject.activeSelf)
        {
            reloadInterface.gameObject.SetActive(false);
            hasMissed = false;

        }


    }
}
