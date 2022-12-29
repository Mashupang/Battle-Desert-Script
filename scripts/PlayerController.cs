using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public string inputID;
    public Transform movePoint;
    public Transform shellSpawnPoint;
    public GameObject shellPrefab;
    private float horizontalInput;
    private float verticalInput;
    private float moveSpeed = 3f;    
    private float minRange = 2f;
    private float maxRangeX = 24f;
    private float maxRangeZ = 14f;
    private float initLevelPosX = 0.2f;
    private float initLevelPosY = 1f;
    private float initLevelPosZ1 = -0.4f;
    private float initLevelPosZ2 = -0.3f;
    private float initLevelPosZ3 = -0.2f;
    private float defaultShellTimer = 1f;
    private int divideShellTimerBy = 2;
    private float minimumShellTimer = 0f;
    private float shellTimer;
    private float turnAngle = 90;
    private int deadEnemies = 0;
    private int level = 0;
    private int levelUpExp = 10;
    private int maxLevel = 3;
    private bool isPlaying;
    private bool isGamePause;
    private Vector3 player1Position = new Vector3(12, 0, 7);
    private Vector3 player2Position = new Vector3(14, 0, 7);
    private List<GameObject> levelList = new List<GameObject>();
    private float safeDistance = 1f;
    private bool holdingKey;
    private bool holdingKey_p1;
    private bool holdingKey_p2;
    private float holdKeyTime = 0.15f;
    private float holdTimer;
    private bool startMoving;
    public bool tankComing;
    private Color p1Color = new Color(0f, 0.5803922f, 1f, 1f);
    private Color p2Color = new Color(0.745283f, 0f, 0f, 1f);
    private float blinkDurduration = 0.05f;
    private int blinkTimes = 10;

    private AudioSource playerAudioSource;
    public AudioClip levelUpClip;

    // Start is called before the first frame update
    void Start()
    {
        playerAudioSource = GetComponent<AudioSource>();

        holdTimer = holdKeyTime;
        movePoint.parent = null;
        
        if (gameObject.name == "Player 1")
        {
            movePoint.transform.position = player1Position;
            holdingKey = holdingKey_p1;
        }
        if (gameObject.name == "Player 2")
        {
            movePoint.transform.position = player2Position;
            holdingKey = holdingKey_p2;
        }
        for(int i = 0; i < maxLevel; i++)
        {
            levelList.Add(this.gameObject.transform.GetChild(i + 1).gameObject);
        }
    }

    void OnEnable()
    {
        shellTimer = defaultShellTimer;
    }

    // Update is called once per frame
    void Update()
    {
        movePoint.GetComponent<SphereCollider>().enabled = (isPlaying) ? true : false ;

        tankComing = movePoint.gameObject.GetComponent<MovePointController>().tankComing;

        holdingKey_p2 = (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D));
        holdingKey_p1 = (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow));
        holdingKey = (gameObject.name == "Player 1") ? holdingKey_p1 : holdingKey_p2;

        if (holdingKey)
        {
            holdTimer -= Time.deltaTime;
            if (holdTimer < 0)
            {
                startMoving = true;
            }
        }
        else
        {
            holdTimer = holdKeyTime;
            startMoving = false;
        }
             
        isGamePause = GameObject.Find("Maze").GetComponent<FindPathAStar>().isGamePause;
        if (!isGamePause)
        {
            PlayerMovement();
        }            

        // Rotate player if horizontalInput = 1 or verticalInput = 1
        if (verticalInput > 0) 
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
            RotateLevelsBadge(1); // go up
        }
        else if (horizontalInput > 0) 
        {
            transform.eulerAngles = new Vector3(0, turnAngle, 0);
            RotateLevelsBadge(2); // go right
        }
        else if (verticalInput < 0) 
        {
            transform.eulerAngles = new Vector3(0, turnAngle * 2, 0);
            RotateLevelsBadge(3); // go down
        }
        else if (horizontalInput < 0) 
        {
            transform.eulerAngles = new Vector3(0, -turnAngle, 0);
            RotateLevelsBadge(4); // go left
        }

        if (Input.GetButtonDown("Fire" + inputID) && shellTimer >= defaultShellTimer && isPlaying == true && !isGamePause)
        {
            Instantiate(shellPrefab, shellSpawnPoint.position, transform.rotation * Quaternion.Euler(0f, 0f, 0f), transform);
            shellTimer = 0.0f;
        }
        else
        {
            shellTimer += Time.deltaTime;
        }
    }

    void RotateLevelsBadge(int playerDirection)
    {
        foreach(GameObject level in levelList)
        {
            level.transform.rotation = Quaternion.Euler(0f, -(gameObject.transform.rotation.y), 0f);
        }
        switch (playerDirection)
        {
            case 1:
                // up
                levelList[0].transform.localPosition = new Vector3(initLevelPosX, initLevelPosY, initLevelPosZ1);
                levelList[1].transform.localPosition = new Vector3(initLevelPosX, initLevelPosY, initLevelPosZ2);
                levelList[2].transform.localPosition = new Vector3(initLevelPosX, initLevelPosY, initLevelPosZ3);
                break;
            case 2:
                // right
                levelList[0].transform.localPosition = new Vector3(-initLevelPosZ1, initLevelPosY, initLevelPosX);
                levelList[1].transform.localPosition = new Vector3(-initLevelPosZ2, initLevelPosY, initLevelPosX);
                levelList[2].transform.localPosition = new Vector3(-initLevelPosZ3, initLevelPosY, initLevelPosX);
                break;
            case 3:
                // down
                levelList[0].transform.localPosition = new Vector3(-initLevelPosX, initLevelPosY, -initLevelPosZ1);
                levelList[1].transform.localPosition = new Vector3(-initLevelPosX, initLevelPosY, -initLevelPosZ2);
                levelList[2].transform.localPosition = new Vector3(-initLevelPosX, initLevelPosY, -initLevelPosZ3);
                break;
            case 4:
                // left
                levelList[0].transform.localPosition = new Vector3(initLevelPosZ1, initLevelPosY, -initLevelPosX);
                levelList[1].transform.localPosition = new Vector3(initLevelPosZ2, initLevelPosY, -initLevelPosX);
                levelList[2].transform.localPosition = new Vector3(initLevelPosZ3, initLevelPosY, -initLevelPosX);
                break;
        }
    }

    void PlayerMovement()
    {

        if (isPlaying)
        {
            CheckBoundaries();
            horizontalInput = Input.GetAxisRaw("Horizontal" + inputID);
            verticalInput = Input.GetAxisRaw("Vertical" + inputID);


            if (tankComing)
            {
                //Vector3 canNotReachPos = movePoint.position;
                //GameObject.Find("Maze").GetComponent<BlackMaskController>().CoverMask(canNotReachPos);

                movePoint.position = new Vector3(Mathf.Round(transform.position.x), 0, Mathf.Round(transform.position.z)); ;
                transform.position = movePoint.position;

            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, movePoint.position, moveSpeed * Time.deltaTime);

                Vector3 playerPos = transform.position;
                Vector3 movePointPos = movePoint.position;
                //GameObject.Find("Maze").GetComponent<BlackMaskController>().PlayersVisibility(playerPos, movePointPos, inputID);
            }


            RaycastHit hit;
            // Ray check for collisions in 1 unit safe distance.             

            if (!Physics.Raycast(movePoint.position, new Vector3(horizontalInput, 0f, verticalInput), out hit, safeDistance) && startMoving)
            {
                if (Vector3.Distance(transform.position, movePoint.position) == 0f)
                {
                    if (Mathf.Abs(horizontalInput) == 1f && Mathf.Abs(verticalInput) != 1f)
                    {
                        movePoint.position += new Vector3(horizontalInput, 0f, 0f);
                    }
                    else if (Mathf.Abs(verticalInput) == 1f && Mathf.Abs(horizontalInput) != 1f)
                    {
                        movePoint.position += new Vector3(0f, 0f, verticalInput);
                    }
                }
            }      
        }
    }

    void CheckBoundaries()
    {
        float xMovementClamp = Mathf.Clamp(movePoint.position.x, minRange,maxRangeX);
        float zMovementClamp = Mathf.Clamp(movePoint.position.z, minRange, maxRangeZ);
        Vector3 limitMovement = new Vector3(xMovementClamp, 0f, zMovementClamp);
        movePoint.position = limitMovement;
    }

    public void CountDeadEnemy()
    {
        deadEnemies++;
        LevelUp();
    }

    void LevelUp()
    {
        if (deadEnemies % levelUpExp == 0 && defaultShellTimer > minimumShellTimer)
        {
            defaultShellTimer /= divideShellTimerBy;
            level++;
            if (level <= maxLevel)
            {
                this.gameObject.transform.GetChild(level).gameObject.SetActive(true);
                StartCoroutine(PlayerPromotedEffect());
                playerAudioSource.clip = levelUpClip;
                playerAudioSource.Play();
            }
        }
    }

    public int CurrentScores()
    {
        return deadEnemies;
    }

    public void PlayerStartPlaying()
    {
        isPlaying = true;
    }
    public void PlayerStopPlaying()
    {
        isPlaying = false;
    }

    IEnumerator PlayerPromotedEffect()
    {
        int blinkTime = 0;
        MeshRenderer[] renderers = gameObject.transform.Find("TankRenderers").GetComponentsInChildren<MeshRenderer>();
        Color blinkColor = new Color(1f, 1f, 1f, 1f);
        Color playerColor = (inputID == "1") ? p1Color : p2Color;

        while (blinkTime < blinkTimes)
        {
            for (int i = 0; i < renderers.Length; i++)
            {
                renderers[i].material.color = blinkColor;
            }
            yield return new WaitForSeconds(blinkDurduration);

            for (int i = 0; i < renderers.Length; i++)
            {
                renderers[i].material.color = playerColor;
            }
            yield return new WaitForSeconds(blinkDurduration);

            blinkTime++;
        }

    }

}
