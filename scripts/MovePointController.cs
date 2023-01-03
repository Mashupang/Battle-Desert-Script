using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePointController : MonoBehaviour
{
    public bool enemyInFront;    
    private string otherPlayerName;
    private List<GameObject> enemyList;
    private bool enemyNear;

    bool CheckNearEnemy()
    {
        for (int i = 0; i < enemyList.Count ; i++)
        {
            if (enemyList[i] == null) { continue; }
            if (Vector3.Distance(transform.position, enemyList[i].transform.position) < 1.5f)
            {
                return true;
            }
        }
        return false;
    }
  
    void Start()
    {
        if (transform.parent != null)
        {
            otherPlayerName = (transform.parent.gameObject.name == "Player 1") ? "Player 2" : "Player 1";
        }
    }

    void Update()
    {
        enemyList = GameObject.Find("Maze").GetComponent<FindPathAStar>().enemyList;
        enemyNear = CheckNearEnemy();
        if (!enemyNear)
        {
            enemyInFront = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Enemy" || other.gameObject.name == otherPlayerName)
        {
            enemyInFront = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Enemy" || other.gameObject.name == otherPlayerName)
        {
            enemyInFront = false;
        }
    }
}
