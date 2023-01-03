using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShellController : MonoBehaviour
{
    public GameObject bustedBasePrefeb;
    public GameObject fireAudioPrefab;
    public GameObject explosionAudioPrefab;
    public AudioClip fireClip;
    public ParticleSystem tankExplosion;
    public ParticleSystem shellExplosion;

    private float shellForce = 1000f; // bullet move speed
    private GameObject playerObject;
    private AudioSource shellAudioSource;

    void Start()
    {
        shellAudioSource = GetComponent<AudioSource>();
        Instantiate(fireAudioPrefab, transform.position, transform.rotation * Quaternion.Euler(0f, 0f, 0f));

        if (transform.parent != null)
        {
            playerObject = transform.parent.gameObject;
            transform.SetParent(null);            
        }
        Rigidbody shellRigidbody = gameObject.GetComponent<Rigidbody>();
        shellRigidbody.AddForce(transform.forward * shellForce);
    }

    void OnCollisionEnter(Collision collision)
    {
        void DestroyBoth()
        {
            Destroy(gameObject);
            Destroy(collision.gameObject);
            Instantiate(explosionAudioPrefab, transform.position, transform.rotation * Quaternion.Euler(0f, 0f, 0f));
        }

        void TankExplosion()
        {
            tankExplosion.transform.SetParent(null);
            tankExplosion.transform.position = collision.gameObject.transform.position;
            tankExplosion.Play();
        }

        void ShellExplosion()
        {
            shellExplosion.transform.SetParent(null);
            shellExplosion.transform.position = collision.gameObject.transform.position;
            shellExplosion.Play();
        }

        if (collision.collider.name == "Boss")
        {
            collision.gameObject.GetComponent<EnemyController>().DamageHP();
            int bossHP = collision.gameObject.GetComponent<EnemyController>().finalWaveHP;

            if (bossHP == 0 && playerObject != null)
            {
                Destroy(collision.gameObject);
                playerObject.GetComponent<PlayerController>().CountDeadEnemy();
                GameObject.Find("Health Canvas").SetActive(false);
                Instantiate(explosionAudioPrefab, transform.position, transform.rotation * Quaternion.Euler(0f, 0f, 0f));
            }
            TankExplosion();
            Destroy(gameObject);
        }

        if (collision.collider.tag == "Enemy" && collision.collider.name != "Boss")
        {
            if (playerObject != null)
            {
                playerObject.GetComponent<PlayerController>().CountDeadEnemy();
            }
            TankExplosion();
            DestroyBoth();
            
        }

        if (collision.collider.tag == "Target")
        {
            Instantiate(bustedBasePrefeb, collision.gameObject.transform.position, transform.rotation * Quaternion.Euler(0f, 0f, 0f));
            ShellExplosion();
            DestroyBoth();           
            if (!GameObject.Find("Maze").GetComponent<FindPathAStar>().CheckbonusRound())
            {
                GameObject.Find("Maze").GetComponent<FindPathAStar>().GameOver();
            }
            
        }

        if (collision.collider.tag == "TargetWall" || collision.collider.tag == "InnerWall")
        {
            ShellExplosion();
            DestroyBoth();
            // update map marker
            GameObject.Find("Maze").GetComponent<Maze>().MarkWallZero(collision.gameObject.transform.position);
        }

        if (collision.collider.tag == "Player")
        {
            //GameObject.Find("Maze").GetComponent<BlackMaskController>().CoverMask(collision.gameObject.transform.position);
            TankExplosion();
            DestroyBoth();
        }

        if (collision.collider.tag == "Shell")
        {            
            TankExplosion();
            DestroyBoth();
        }

        if (collision.collider.tag == "Wall")
        {
            Destroy(gameObject);
        }
    }
}
