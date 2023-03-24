using Photon.Pun;
using UnityEngine;

public class Weapon : MonoBehaviourPunCallbacks
{
    #region Variables

    [SerializeField] private Gun[] loadOut;
    [SerializeField] private Transform weaponPanrent;
    [SerializeField] private GameObject bulletHolePerfab;
    [SerializeField] private LayerMask canBeShot;


    private float currentCooldown;
    private int currentIndex;
    private GameObject currentWeapon;

    #endregion

    #region MonoBehaviour Callbacks

    // Update is called once per frame
    private void Update()
    {
        if (!photonView.IsMine) return;

        if (Input.GetKeyDown(KeyCode.Alpha1)) Equip(0);

        if (currentWeapon != null)
        {
            Aim(Input.GetMouseButton(1));


            if (Input.GetMouseButton(0) && currentCooldown <= 0)
            {
                Shoot();
            }

            currentWeapon.transform.localPosition = Vector3.Lerp(currentWeapon.transform.localPosition, Vector3.zero, Time.deltaTime * 4f);

            //cooldown
            if (currentCooldown > 0) currentCooldown -= Time.deltaTime;
        }

    }

    #endregion


    #region Private Methods
    private void Equip(int index)
    {
        if (currentWeapon != null) Destroy(currentWeapon);

        currentIndex = index;

        GameObject newWeapon = Instantiate(loadOut[index].prefab, weaponPanrent.position, weaponPanrent.rotation, weaponPanrent) as GameObject;
        newWeapon.transform.localPosition = Vector3.zero;
        newWeapon.transform.localEulerAngles = Vector3.zero;
        newWeapon.GetComponent<Sway>().enabled = photonView.IsMine;

        currentWeapon = newWeapon;
    }

    private void Aim(bool isAiming)
    {
        Transform anchor = currentWeapon.transform.Find("Anchor");
        Transform StacteADS = currentWeapon.transform.Find("States/ADS");
        Transform StacteHip = currentWeapon.transform.Find("States/Hip");

        if (isAiming)
        {
            anchor.position = Vector3.Lerp(anchor.position, StacteADS.position, Time.deltaTime * loadOut[currentIndex].aimSpeed);
        }
        else
        {
            anchor.position = Vector3.Lerp(anchor.position, StacteHip.position, Time.deltaTime * loadOut[currentIndex].aimSpeed);

        }
    }

    private void Shoot()
    {
        Transform spawn = transform.Find("Cameras/Normal Camera");

        //bloom
        Vector3 bloom = spawn.position + spawn.forward * 1000f;
        bloom += Random.Range(-loadOut[currentIndex].bloom, loadOut[currentIndex].bloom) * spawn.up;
        bloom += Random.Range(-loadOut[currentIndex].bloom, loadOut[currentIndex].bloom) * spawn.right;
        bloom -= spawn.position;
        bloom.Normalize();

        //raycast
        RaycastHit hit = new RaycastHit();

        if (Physics.Raycast(spawn.position, bloom, out hit, 1000f, canBeShot))
        {
            GameObject newHole = Instantiate(bulletHolePerfab, hit.point + hit.normal * 0.001f, Quaternion.identity) as GameObject;
            newHole.transform.LookAt(hit.point + hit.normal);
            Destroy(newHole, 5f);

        }

        //Gun fix
        currentWeapon.transform.Rotate(-loadOut[currentIndex].recoil, 0, 0);
        currentWeapon.transform.position -= currentWeapon.transform.forward * loadOut[currentIndex].kickback;

        //cooldown
        currentCooldown = loadOut[currentIndex].firerate;
    }
    #endregion
}
