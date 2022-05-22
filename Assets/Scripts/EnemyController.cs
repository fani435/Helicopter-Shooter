using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float Health;
    public float RespawnTime;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.CompareTag("Bullet"))
        {
            //Hiding the bullet since we have been hit
            collision.transform.gameObject.SetActive(false);
            TakeDamage();
        }
    }

    public void TakeDamage()
    {
        Health -= 1;

        if (Health == 0)
        {
            //Hiding the visible red cube
            GetComponent<MeshRenderer>().enabled = false;
            //Disabling the collider
            GetComponent<BoxCollider>().enabled = false;
            //Starting this coroutine so enemy can respawn after some fixed time
            StartCoroutine(RespawnAfterDelay());
        }
    }

    IEnumerator RespawnAfterDelay()
    {
        yield return new WaitForSeconds(RespawnTime);
        //Restoring health of enemy
        Health = 3f;
        //Showing the enemy again
        GetComponent<MeshRenderer>().enabled = true;
        //Enabling the collider
        GetComponent<BoxCollider>().enabled = true;
    }

}
