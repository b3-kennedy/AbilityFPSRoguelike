using UnityEngine;

public class Vacuum : Ability
{
    Transform firePoint;
    public GameObject projectile;
    public float projectileForce;
    Transform cam;
    public LayerMask layerMask;
    Vector3 hitPoint;

    public override void OnInitialise()
    {

        firePoint = GetCaster().transform.Find("CameraHolder/Recoil/Camera/GunPosition/GunParent/Gun/FirePoint");
        cam = GetCaster().transform.Find("CameraHolder/Recoil/Camera");
    }

    public override void Cast()
    {
        if(Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit hit, 1000, layerMask))
        {
            if (hit.collider) 
            {
                hitPoint = hit.point;
            }
            else
            {
                hitPoint = cam.transform.forward * 1000;
            }

            //Vector3 dir = (hitPoint - );
            GameObject spawnedProjectile = Instantiate(projectile, firePoint.position, Quaternion.identity);
            //spawnedProjectile.GetComponent<Rigidbody>().AddForce();
        }


    }
}
