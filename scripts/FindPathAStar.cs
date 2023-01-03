/* For more info on 'The A* Algorithm', check out link below
learn.unity.com/project/a-36369ng */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class PathMarker {

    public MapLocation location;
    public float G, H, F;
    public GameObject marker;
    public PathMarker parent;

    public PathMarker(MapLocation l, float g, float h, float f, GameObject m, PathMarker p) {

        location = l;
        G = g;
        H = h;
        F = f;
        marker = m;
        parent = p;
    }

    public override bool Equals(object obj) {

        if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            return false;
        else
            return location.Equals(((PathMarker)obj).location);
    }

    public override int GetHashCode() {

        return 0;
    }
}

public class FindPathAStar : MonoBehaviour {    
    public Maze maze; // maze (game map) size is 27 x 17, invisible range is 2 on each side
    public GameObject start;
    public GameObject end;
    public GameObject pathP;
    public GameObject player_1;
    public GameObject player_2;
    public GameObject enemyPrefab;
    public GameObject gameoverCanvas; // canvas to display game over, etc. 
    public GameObject resultCanvas; // canvas to display scores, etc. 
    public GameObject winnerCanvas; // canvas to display winner, etc. 
    public GameObject bgMusic;
    List<Vector3> paths = new List<Vector3>(); // from enemy's start position to target position (home base)
    List<PathMarker> open = new List<PathMarker>(); // open markers for searching paths
    List<PathMarker> closed = new List<PathMarker>(); // closed markers for searching paths 
    public List<GameObject> enemyList = new List<GameObject>(); // list of enemies
    public int waveCount = 0; // (0 based) round 1, which round the game is currently on
    public int finalRound = 2; // (0 based) 3 rounds of emenies in total
    public bool isGamePause = true; 
    public bool isGameOver;
    public bool player_1_wins;
    public bool player_2_wins;
    public TextMeshProUGUI roundText; // which round is completed
    public TextMeshProUGUI playerScoreText_1;
    public TextMeshProUGUI playerScoreText_2;
    public TextMeshProUGUI blinkingText; // display text "Go!" in the beginning of each round
    public TextMeshProUGUI winnerText;
    public AudioClip countingClip;
    public AudioClip winClip;
    public AudioClip gameOverClip;
    public AudioClip gogogoClip;
    public AudioClip weaponsFreeClip;
    public AudioClip goodJobClip;
    public AudioClip perfectClip;
    public AudioClip allClearClip;
    public ParticleSystem confetti;

    private int spawnEnemiesRound_1 = 19; // (0 based) spawn 20 emenies in round 1
    private int spawnEnemiesRound_2 = 19; // (0 based) spawn 20 emenies in round 2
    private int enemiesToSpawnIndex = 0;
    private int spawnPosX;
    private int spawnPosZ;
    private int minSpawnRange = 2; // min range for X and Z axis
    private int maxSpawnRangeX = 25;
    private int maxSpawnRangeZ = 15;
    private int targetPositionX = 13; // home base X axis
    private int targetPositionZ = 8; // home base Z axis
    private float bufferDuration = 2f; // used to have a delay whilst the round starts or ends.
    private float beforeCountScoresWaitTime;
    private float afterCountScoresWaitTime;
    private float countScoreSpeed = 5f;
    private float playerDisplayScore_1 = 0f;
    private float playerDisplayScore_2 = 0f;
    private bool beforeCountScoresTimerRunning;
    private bool afterCountScoresTimerRunning;
    private bool bonusRound;
    private bool done = false; // searching path
    private bool playedWinSoundOnce;
    private bool playedGameOverSoundOnce;
    private bool playedWinMusic;
    private bool playingHonorSound;
    private AudioSource mazeAudioSource;


    void Start()
    {
        mazeAudioSource = GetComponent<AudioSource>();
        ResetTimer(); // delay timer
        StartCoroutine(NextSpawnEnemies(spawnEnemiesRound_1));
    }

    void RemoveAllMarkers() {

        GameObject[] markers = GameObject.FindGameObjectsWithTag("marker");

        foreach (GameObject m in markers) Destroy(m);
    }

    void BeginSearch(int startX,int startZ, PathMarker startNode, PathMarker endNode, PathMarker lastPos) {

        done = false;
        RemoveAllMarkers();

        open.Clear();
        closed.Clear();

        open.Add(startNode);
        lastPos = startNode;

        while (!done)
        {
            Search(lastPos);            
        }
        if (done)
        {
            GetPath();            
            done = false;         
        }

        void Search(PathMarker thisNode)
        {
            if (thisNode == null) return;
            if (thisNode.Equals(endNode))
            {
                // DONE Searching!
                done = true;
                return;
            }

            foreach (MapLocation dir in maze.directions)
            {

                MapLocation neighbour = dir + thisNode.location;

                if (maze.map[neighbour.x, neighbour.z] == 1) continue;
                if (neighbour.x < 1 || neighbour.x >= maze.width || neighbour.z < 1 || neighbour.z >= maze.depth) continue;
                if (IsClosed(neighbour)) continue;

                float g = Vector2.Distance(thisNode.location.ToVector(), neighbour.ToVector()) + thisNode.G;
                float h = Vector2.Distance(neighbour.ToVector(), endNode.location.ToVector());
                float f = g + h;

                GameObject pathBlock = Instantiate(pathP, new Vector3(neighbour.x * maze.scale, 0.0f, neighbour.z * maze.scale), Quaternion.identity);

                if (!UpdateMarker(neighbour, g, h, f, thisNode))
                {
                    open.Add(new PathMarker(neighbour, g, h, f, pathBlock, thisNode));
                }
            }
            open = open.OrderBy(p => p.F).ThenBy(n => n.H).ToList<PathMarker>();
            PathMarker pm = (PathMarker)open.ElementAt(0);
            closed.Add(pm);
            open.RemoveAt(0);
            lastPos = pm;
        }

        void GetPath()
        {
            RemoveAllMarkers();

            paths.Add(new Vector3(lastPos.location.x, 0f, lastPos.location.z));

            while (!startNode.Equals(lastPos) && lastPos != null)
            {
                Instantiate(pathP, new Vector3(lastPos.location.x * maze.scale, 0.0f, lastPos.location.z * maze.scale), Quaternion.identity);
                lastPos = lastPos.parent;
                paths.Add(new Vector3(lastPos.location.x, 0f, lastPos.location.z));
            }

            Instantiate(pathP, new Vector3(startNode.location.x * maze.scale, 0.0f, startNode.location.z * maze.scale), Quaternion.identity);
            paths.Reverse();

            enemyList[enemiesToSpawnIndex].GetComponent<EnemyController>().paths.AddRange(paths); // enemy gets a list of paths to the base

            paths.Clear();
            RemoveAllMarkers();           

        }

    }

    bool UpdateMarker(MapLocation pos, float g, float h, float f, PathMarker prt) {

        foreach (PathMarker p in open) {

            if (p.location.Equals(pos)) {

                p.G = g;
                p.H = h;
                p.F = f;
                p.parent = prt;
                return true;
            }
        }
        return false;
    }

    bool IsClosed(MapLocation marker) {

        foreach (PathMarker p in closed) {

            if (p.location.Equals(marker)) return true;
        }
        return false;
    }

    void SpawnEnemy(int enemiesToSpawnIndex)
    {      
        int spawnRandomPosX = Random.Range(minSpawnRange, maxSpawnRangeX);
        int spawnRandomPosZ = Random.Range(minSpawnRange, maxSpawnRangeZ);
        int spawnDirection = Random.Range(1, 5);

        switch (spawnDirection)
        {
            case 1:
                // top
                spawnPosX = spawnRandomPosX;
                spawnPosZ = maxSpawnRangeZ;
                break;
            case 2:
                // right
                spawnPosX = maxSpawnRangeX;
                spawnPosZ = spawnRandomPosZ;
                break;
            case 3:
                // bottom
                spawnPosX = spawnRandomPosX;
                spawnPosZ = 1;
                break;
            case 4:
                // left
                spawnPosX = 1;
                spawnPosZ = spawnRandomPosZ;
                break;
        }

        Vector3 spawnLocation = new Vector3(spawnPosX, 0.0f, spawnPosZ);
        enemyList.Add((GameObject)Instantiate(enemyPrefab, spawnLocation, enemyPrefab.transform.rotation));  // spawn enemies
        if (waveCount == finalRound)
        {
            enemyList[enemiesToSpawnIndex].name = "Boss";
        }
        else
        {
            enemyList[enemiesToSpawnIndex].name = "Enemy(" + enemiesToSpawnIndex + ")";
        }

        List<MapLocation> locations = new List<MapLocation>();
        locations.Clear();

        for (int z = 1; z < maze.depth - 1; ++z)
        {
            for (int x = 1; x < maze.width - 1; ++x)
            {
                // walkable (maze.map[x, z] = 0)
                if (maze.map[x, z] != 1)
                {
                    locations.Add(new MapLocation(x, z));
                }
            }
        }

        int startIndex = locations.IndexOf(new MapLocation(spawnPosX, spawnPosZ));
        int endIndex = locations.IndexOf(new MapLocation(targetPositionX, targetPositionZ));

        Vector3 startLocation = new Vector3(locations[startIndex].x * maze.scale, 0.0f, locations[startIndex].z * maze.scale);
        PathMarker startNode = new PathMarker(new MapLocation(locations[startIndex].x, locations[startIndex].z), 0.0f, 0.0f, 0.0f, Instantiate(start, startLocation, Quaternion.identity), null);
        Vector3 endLocation = new Vector3(locations[endIndex].x * maze.scale, 0.0f, locations[endIndex].z * maze.scale);
        PathMarker endNode = new PathMarker(new MapLocation(locations[endIndex].x, locations[endIndex].z), 0.0f, 0.0f, 0.0f, Instantiate(end, endLocation, Quaternion.identity), null);
        PathMarker lastPos = startNode;

        BeginSearch(spawnPosX, spawnPosZ, startNode, endNode, lastPos);
    }

    void CheckGameStateButton()
    {
        // for selecting buttons by default
        if(player_1_wins || player_2_wins)
        {
            GameObject replayButton = winnerCanvas.transform.Find("Button Panel/Replay Button").gameObject;
            EventSystem.current.SetSelectedGameObject(replayButton);
        }
        else if (isGameOver)
        {
            GameObject replayButton = gameoverCanvas.transform.Find("Panel/Replay Button").gameObject;
            EventSystem.current.SetSelectedGameObject(replayButton);
        }
    }

    void ShowConfetti()
    {
        confetti.Play();
        winnerCanvas.SetActive(true);
        StartCoroutine(FadeCanvas(winnerCanvas, 0f, 1f, 2f));

        //HideMask(); 
        WinSound();

        if (!playedWinMusic)
        {
            bgMusic.GetComponent<BackgroundMusicController>().PlayWinMusic();
            playedWinMusic = true;
        }
    }

    // this function is not used in the game for now 
    void HideMask()
    {
        GameObject[] masks;
        masks = GameObject.FindGameObjectsWithTag("Mask");
        foreach (GameObject mask in masks)
        {
            mask.gameObject.SetActive(false);
        }
    }

    void CheckDeadTanks()
    {
        for (int i = 0; i < enemyList.Count; i++)
        {
            if (enemyList[i] == null)
            {
                enemyList.RemoveAt(i);                
            }
        }

        if (bonusRound && player_1 != null && player_2 != null)
        {
            // bonus round, PVP
            player_1.GetComponent<PlayerController>().PlayerStartPlaying();
            player_2.GetComponent<PlayerController>().PlayerStartPlaying();
        }
        else
        {
            // round ends, starts counting scores and spawning enemies
            CountPlusSpawn();
        }

        if ((player_1 == null || player_2 == null) && !bonusRound)
        {
            // either p1 or p2 is dead
            GameOver();
        }
        else if ((player_1 == null && player_2 == null) && bonusRound)
        {
            // die together in final (bonus round)
            GameOver();
        }
        else if ((player_1 == null || player_2 == null) && bonusRound)
        {
            // remove remaining shells in the map
            RemoveAllShells();

            isGamePause = true;
            string winner;
            string playerColor;
            winner = (player_1 == null) ? "P2" : "P1";
            playerColor = (player_1 == null) ? "#BE0000" : "#0094FF";

            winnerText.text = $"<color={playerColor}>{winner}</color> Wins!";

            if (winner == "P1")
            {
                player_1_wins = true;
            }
            else
            {
                player_2_wins = true;
            }          
            ShowConfetti();
        }

    }

    void CountPlusSpawn()
    {
        if (enemyList.Count == 0 && !isGameOver && player_1 != null && player_2 != null)
        {
            CountPlayersScores();

            int playerCurrentScores_1 = player_1.GetComponent<PlayerController>().CurrentScores();
            int playerCurrentScores_2 = player_2.GetComponent<PlayerController>().CurrentScores();

            // done counting scores
            if (playerCurrentScores_1 == (int)playerDisplayScore_1 && playerCurrentScores_2 == (int)playerDisplayScore_2)
            {
                RunAfterCountScoresTimer();

                if (afterCountScoresWaitTime <= 0)
                {

                    if (waveCount == finalRound && player_1.activeInHierarchy && player_2.activeInHierarchy)
                    {
                        // starting PVP
                        bonusRound = true;
                        resultCanvas.GetComponent<CanvasGroup>().alpha = 0;
                        StartCoroutine(BlinkingText("Final"));
                    }
                    else if (waveCount == finalRound)
                    {
                        // for single player mode
                        resultCanvas.GetComponent<CanvasGroup>().alpha = 0;
                        winnerText.text = $"<color=#0094FF>P1</color> Wins!";
                        player_1_wins = true;
                        ShowConfetti();
                    }
                    else
                    {
                        waveCount++;
                        //GetComponent<BlackMaskController>().NextStage();

                        enemiesToSpawnIndex = 0;
                        if (waveCount == finalRound)
                        {
                            SpawnEnemy(enemiesToSpawnIndex); // spawn a boss
                        }
                        else
                        {
                            StartCoroutine(NextSpawnEnemies(spawnEnemiesRound_2)); // spawn enemies for round 2 
                        }                        

                        player_1.GetComponent<PlayerController>().PlayerStartPlaying();
                        player_2.GetComponent<PlayerController>().PlayerStartPlaying();
                        resultCanvas.SetActive(false);
                        StartBlinking();
                        ResetTimer();
                        playingHonorSound = false;
                    }
                }
            }                      
        }
    }

    void CountPlayersScores()
    {
        player_1.GetComponent<PlayerController>().PlayerStopPlaying();
        player_2.GetComponent<PlayerController>().PlayerStopPlaying();

        RunBeforeCountScoresTimer();

        if (beforeCountScoresWaitTime <= 1 && !playingHonorSound)
        {
            // says 'good job' etc. before counting
            PlayHonorSound();
        }

        if (beforeCountScoresWaitTime <= 0)
        {
            resultCanvas.SetActive(true);

            if (!player_2.activeInHierarchy)
            {
                // No P2
                GameObject p2Result = resultCanvas.transform.Find("Panel/Result Body/Right Col BG").gameObject;
                p2Result.SetActive(false);
            }

            if (player_1 != null)
            {
                float playerCurrentScores_1 = (float)(player_1.GetComponent<PlayerController>().CurrentScores());
                if (playerDisplayScore_1 < playerCurrentScores_1)
                {
                    int nextAddNum = (int)playerDisplayScore_1 + 1;
                    playerDisplayScore_1 += countScoreSpeed * Time.deltaTime;
                    int displayScore_1 = (int)playerDisplayScore_1;
                    playerScoreText_1.text = displayScore_1.ToString();
                    if (playerDisplayScore_1 >= nextAddNum)
                    {
                        CountingSound();
                        nextAddNum++;
                    }
                }
            }            

            if (player_2 != null)
            {
                float playerCurrentScores_2 = (float)(player_2.GetComponent<PlayerController>().CurrentScores());
                if (playerDisplayScore_2 < playerCurrentScores_2)
                {
                    int nextAddNum = (int)playerDisplayScore_2 + 1;
                    playerDisplayScore_2 += countScoreSpeed * Time.deltaTime;
                    int displayScore_2 = (int)playerDisplayScore_2;
                    playerScoreText_2.text = displayScore_2.ToString();
                    if (playerDisplayScore_2 >= nextAddNum)
                    {
                        CountingSound();
                        nextAddNum++;
                    }
                }
            }

            int roundConut = waveCount + 1;
            roundText.text = $"Round <color=white>{ roundConut }</color> Complete!";
        }       
    }

    void PlayHonorSound()
    {
        if (waveCount == 0)
        {
            mazeAudioSource.clip = goodJobClip;
        }
        else if (waveCount == 1)
        {
            mazeAudioSource.clip = perfectClip;
        }
        else if (waveCount == 2)
        {
            mazeAudioSource.clip = allClearClip;
        }
        mazeAudioSource.Play();
        playingHonorSound = true;
    }

    void RemoveAllShells()
    {
        GameObject[] shells;
        shells = GameObject.FindGameObjectsWithTag("Shell");
        foreach (GameObject shell in shells) Destroy(shell);
    }

    void ResetTimer()
    {
        beforeCountScoresWaitTime = bufferDuration;
        afterCountScoresWaitTime = bufferDuration;
    }
    
    void RunBeforeCountScoresTimer()
    {        
        beforeCountScoresTimerRunning = true;
        if (beforeCountScoresTimerRunning)
        {
            beforeCountScoresWaitTime = (beforeCountScoresWaitTime > 0) ? beforeCountScoresWaitTime - Time.deltaTime : 0;
            beforeCountScoresTimerRunning = (beforeCountScoresWaitTime == 0) ? false : true;
        }
    }

    void RunAfterCountScoresTimer()
    {
        afterCountScoresTimerRunning = true;
        if (afterCountScoresTimerRunning)
        {
            afterCountScoresWaitTime = (afterCountScoresWaitTime > 0) ? afterCountScoresWaitTime - Time.deltaTime : 0;
            afterCountScoresTimerRunning = (afterCountScoresWaitTime == 0) ? false : true;
        }
    }

    public void GameOver()
    {
        HideMask();
        gameoverCanvas.SetActive(true);        
        isGameOver = true;
        StartCoroutine(FadeCanvas(gameoverCanvas, 0f, 1f, 1f));
        isGamePause = true;
        GameOverSound();
        bgMusic.GetComponent<BackgroundMusicController>().StopMusic();
    }

    IEnumerator FadeCanvas(GameObject canvas, float fadeFrom, float fadeTo, float duration)
    {
        float elapsedTime = 0;
        bool fading = true;
        while (canvas.GetComponent<CanvasGroup>().alpha < duration && fading)
        {
            canvas.GetComponent<CanvasGroup>().alpha = Mathf.MoveTowards(fadeFrom, fadeTo, elapsedTime / duration);
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }
        canvas.GetComponent<CanvasGroup>().alpha = (int)fadeTo;
        fading = false;
    }

    IEnumerator NextSpawnEnemies(int numberOfEnemies)
    {
        // 0 based
        while (enemiesToSpawnIndex != (numberOfEnemies + 1))
        {            
            SpawnEnemy(enemiesToSpawnIndex);
            enemiesToSpawnIndex++;
            yield return null;
        }
    }

    IEnumerator BlinkingText(string text = "Go!")
    {
        mazeAudioSource.clip = (bonusRound) ? weaponsFreeClip : gogogoClip;
        mazeAudioSource.Play();

        int blinkTime = 0;
        while (blinkTime < 3)
        {
            blinkingText.text = text;
            yield return new WaitForSeconds(0.15f);
            blinkingText.text = "";
            yield return new WaitForSeconds(0.15f);
            blinkTime++;
        }
    }

    public void StartBlinking()
    {
        StartCoroutine(BlinkingText());
    }

    void ReplayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public bool CheckbonusRound()
    {
        return bonusRound;
    }

    public void StartPlaying()
    {
        isGamePause = false;
    }

    void CountingSound()
    {
        mazeAudioSource.clip = countingClip;
        mazeAudioSource.Play();
    }

    void WinSound()
    {        
        if (!mazeAudioSource.isPlaying && !playedWinSoundOnce)
        {
            mazeAudioSource.clip = winClip;
            mazeAudioSource.Play();
            playedWinSoundOnce = true;
        }
    }

    void GameOverSound()
    {
        if (!mazeAudioSource.isPlaying && !playedGameOverSoundOnce)
        {
            mazeAudioSource.clip = gameOverClip;
            mazeAudioSource.Play();
            playedGameOverSoundOnce = true;
        }
    }

    void Update() {
        CheckDeadTanks();
        CheckGameStateButton();
    }

}