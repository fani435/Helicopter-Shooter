using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingManager : MonoBehaviour
{
    public GameObject Bullet;
    public float BulletSpeed;
    public float TimeBetweenBullets;

    public bool CanShoot;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        //Can helicopter shoot yet
        if(CanShoot)
        {
            //If the object in range is an enemy
            if (other.CompareTag("Enemy"))
            {
                Debug.Log("Found enemy");
                //Preventing helicopter from shooting again until TimeBetweenBullets has passed
                CanShoot = false;
                Shoot(other.transform);
            }
        }
    }

    public void Shoot(Transform Target)
    {
        //Moving bullet to the barrel of the gun
        Bullet.transform.position = transform.position;
        //Enabing trail renderer of bullet
        Bullet.GetComponent<TrailRenderer>().enabled = true;
        //Point bullet to target
        Bullet.transform.LookAt(Target);
        //Show bullet
        Bullet.SetActive(true);
        //Shoot bullet
        Bullet.GetComponent<Rigidbody>().velocity = Bullet.transform.forward * BulletSpeed;
        //Wait for a moment before shooting again
        StartCoroutine(WaitForTimeBetweenBullets());
    }

    IEnumerator WaitForTimeBetweenBullets()
    {
        yield return new WaitForSeconds(TimeBetweenBullets);
        CanShoot = true;
    }
}
