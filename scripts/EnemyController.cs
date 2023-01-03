using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{
    public int finalWaveHP; // boss HP
    public GameObject shellPrefab;
    public Transform shellSpawnPoint;
    public AudioClip damageClip;
    public List<Vector3> paths = new List<Vector3>(); // paths to the home base

    private int bossHP = 20;
    private int maxfinalWaveHP;
    private int currentWave;
    private int selfIndexOfEnemyList;
    private int pathCount;
    private float moveSpeed;
    private float minMoveSpeed = 0.5f;
    private float shellTimer; // wait time to shoot next bullet
    private float defaultShellTimer = 3.0f; 
    private float enemyColorRGB;
    private bool gameIsPause;
    private bool isMoving;
    private bool insideMap; // visible for players
    private bool enemyInTheWay; // another enemy tank in front of an enemy 
    private Vector3 bossHpPos = new Vector3(0, 1, 0.7f);
    private ParticleSystem dustTrail;
    private AudioSource enemyAudioSource;
    private List<GameObject> enemyList;

    Coroutine moveCoroutine;
    RaycastHit closestHit;
    RaycastHit[] hits;

    void OnEnable()
    {
        shellTimer = defaultShellTimer; // enemy is ready to fire a bullet 
    }

    void Start()
    {
        enemyAudioSource = GetComponent<AudioSource>();
        currentWave = GameObject.Find("Maze").GetComponent<FindPathAStar>().waveCount;
        moveSpeed = minMoveSpeed + (minMoveSpeed * currentWave); //  different moving speed 
        defaultShellTimer = defaultShellTimer - currentWave; //  different firing rate
        pathCount = paths.Count;
        StartCoroutine(MoveEnemy());
        isMoving = true;
        finalWaveHP = bossHP;
        maxfinalWaveHP = bossHP;        
        enemyColorRGB = 0.5f - (0.25f * currentWave); // enemy's color from grey to black
        MeshRenderer[] renderers = gameObject.transform.Find("TankRenderers").GetComponentsInChildren<MeshRenderer>();
        dustTrail = transform.GetChild(2).GetComponentInChildren<ParticleSystem>();
        
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material.color = new Color(enemyColorRGB, enemyColorRGB, enemyColorRGB, 1.0f);
        }
    }

    void Update()
    {
        enemyList = GameObject.Find("Maze").GetComponent<FindPathAStar>().enemyList;
        gameIsPause = GameObject.Find("Maze").GetComponent<FindPathAStar>().isGamePause;
        selfIndexOfEnemyList = enemyList.IndexOf(gameObject);
        isMoving = CheckSafeDistance() && !gameIsPause;
        insideMap = InsideMap();
        var emissionModule = dustTrail.emission;
        emissionModule.enabled = (insideMap) ? true : false;
        RaycastHit[] hits = Physics.RaycastAll(transform.position, transform.forward, 25.0f);

        if (hits.Length != 0 && hits[0].collider != null)
        {
            closestHit = hits[0];
        }

        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].distance <= closestHit.distance)
            {
                closestHit = hits[i];
                enemyInTheWay = (closestHit.transform.tag == "Enemy") ? true : false;
            }
        }

        // 3:Enemy, 6: Player, 7:TargetWall, 8:Target 
        int layerMaskCombined = (1 << 3) | (1 << 6) | (1 << 7) | (1 << 8);
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 2f, layerMaskCombined))
        {
            if (hit.transform.tag == "Player" || hit.transform.tag == "TargetWall" || hit.transform.tag == "Target")
            {
                //Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.red);
                EnemyFire();
                isMoving = false;
            }
        }
        else if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 23f, layerMaskCombined))
        {
            if (hit.transform.tag == "Player" || hit.transform.tag == "TargetWall" || hit.transform.tag == "Target")
            {
                //Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
                EnemyFire();
            }
        }        
        else
        {
            //Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 25, Color.green);           
        }
        shellTimer += Time.deltaTime;

        if(gameObject.name == "Boss")
        {            
            GameObject healthCanvas = GameObject.Find("Health Canvas");            
            healthCanvas.transform.localPosition = transform.localPosition + bossHpPos; // HP bar is right above the boss
            healthCanvas.GetComponent<CanvasGroup>().alpha = (insideMap) ? 1 : 0;
        }
    }

    void EnemyFire()
    {
        if (shellTimer >= defaultShellTimer && !enemyInTheWay && !gameIsPause && insideMap)
        {
            Instantiate(shellPrefab, shellSpawnPoint.position, transform.rotation * Quaternion.Euler(0f, 0f, 0f));
            shellTimer = 0.0f;
        }

    }

    bool CheckSafeDistance()
    {
        // prevent enemy tanks from overlapping each other
        for (int i = 0; i < enemyList.Count; i++)
        {
            if (enemyList[i] == null) { continue; }
            else if (Vector3.Distance(transform.position, enemyList[i].transform.position) < 2.0f && enemyList[i].GetComponent<EnemyController>().pathCount < pathCount)
            {
                // enemy tank will stop moving under these conditions have met:
				// 1. too close to each other
				// 2. other enemy tanks are closer to the home base
                return false;
            }
            else if (enemyList[i].GetComponent<EnemyController>().pathCount == pathCount && i < selfIndexOfEnemyList)
            {
                // enemy tank will also stop moving when
                // 1. other enemy tanks have same distance between current pos and the base pos
                // 2. other enemy tanks have lower index numbers
                return false;
            }
        }

        return true;
    }

    bool InsideMap()
    {
        bool insideMapBool;
        // visible maze size is 23 x 13, invisible range is 2 on each side
        insideMapBool = (transform.position.x >= 2 && transform.position.x <= 24 && transform.position.z <= 14 && transform.position.z >= 2) ? true : false;
        return insideMapBool;
    }

    IEnumerator MoveEnemy()
    {
        for (int i = 0; i < paths.Count; i++)
        {             
            moveCoroutine = StartCoroutine(Moving(i));
            yield return moveCoroutine;
            pathCount--;            
        }
    }

    IEnumerator Moving(int currentPosition)
    {
        while (transform.position != paths[currentPosition])
        {
            if (paths[currentPosition].x > transform.position.x)
            {
                transform.eulerAngles = new Vector3(0, 90, 0);
            }
            else if (paths[currentPosition].x < transform.position.x)
            {
                transform.eulerAngles = new Vector3(0, -90, 0);
            }
            else if (paths[currentPosition].z > transform.position.z)
            {
                transform.eulerAngles = new Vector3(0, 0, 0);               
            }
            else if (paths[currentPosition].z < transform.position.z)
            {
                transform.eulerAngles = new Vector3(0, 180, 0);
            }
            transform.position = Vector3.MoveTowards(transform.position, paths[currentPosition], moveSpeed * Time.deltaTime);
            yield return new WaitUntil(() => isMoving == true);
        }
    }

    public void DamageHP()
    {
        // for boss only
        finalWaveHP--;
        Image greenHealthBar = GameObject.Find("Health Canvas").transform.GetChild(1).gameObject.GetComponent<Image>();
        greenHealthBar.fillAmount = (float)finalWaveHP / (float)maxfinalWaveHP;
        enemyAudioSource.clip = damageClip;
        enemyAudioSource.Play();
    }

}