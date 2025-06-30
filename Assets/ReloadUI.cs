using System.Net;
using UnityEditor.Rendering;
using UnityEngine;

public class ReloadUI : MonoBehaviour
{
    public GameObject reloadUIParent;
    public GameObject reloadBar;
    public GameObject ammoCount;
    Gun gun;
    float timer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gun = GetComponent<Gun>();
    }

    // Update is called once per frame
    void Update()
    {
        if (gun.IsReloading())
        {
            reloadUIParent.SetActive(true);
            ammoCount.SetActive(false);
            timer += Time.deltaTime;
            float t = timer / gun.gunData.reloadTime;
            t = Mathf.Clamp01(t);
            reloadBar.transform.localScale = new Vector2(Mathf.Lerp(0, 1, t), reloadBar.transform.localScale.y);
        }
        else if(!gun.IsReloading() && reloadUIParent.activeSelf)
        {
            reloadUIParent.SetActive(false);
            ammoCount.SetActive(true);
            timer = 0;
        }
    }
}
