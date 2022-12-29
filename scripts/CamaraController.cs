using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamaraController : MonoBehaviour
{
    Camera m_Camera;
    //visible screen size is 23 x 13
    private float zoomOutSpeed = 20.0f;
    private float zoomInSpeed = 1.0f;
    private float zoomInSize = 1.5f;
    private Vector3 cameraTopDownPos = new Vector3(13, 13, 8);
    private Vector3 betweenPlayersPos = new Vector3(13, 13, 7);
    private float desiredHalfHeight = 6.5f;

    void Start()
    {
        m_Camera = GetComponent<Camera>();
    }

    void Update()
    {       
        // set the desired aspect ratio (the values in this example are
        // hard-coded for 16:9, but you could make them into public
        // variables instead so you can set them at design time)
        float targetaspect = 23.0f / 13.0f;

        // determine the game window's current aspect ratio
        float windowaspect = (float)Screen.width / (float)Screen.height;

        // current viewport height should be scaled by this amount
        float scaleheight = windowaspect / targetaspect;

        // obtain camera component so we can modify its viewport
        Camera camera = GetComponent<Camera>();

        // if scaled height is less than current height, add letterbox
        if (scaleheight < 1.0f)
        {
            Rect rect = camera.rect;

            rect.width = 1.0f;
            rect.height = scaleheight;
            rect.x = 0;
            rect.y = (1.0f - scaleheight) / 2.0f;

            camera.rect = rect;
        }
        else // add pillarbox
        {
            float scalewidth = 1.0f / scaleheight;

            Rect rect = camera.rect;

            rect.width = scalewidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scalewidth) / 2.0f;
            rect.y = 0;

            camera.rect = rect;
        }


        if (!GameObject.Find("Maze").GetComponent<FindPathAStar>().player_1_wins && !GameObject.Find("Maze").GetComponent<FindPathAStar>().player_2_wins)
        {
            if (!GameObject.Find("Maze").GetComponent<FindPathAStar>().isGamePause)
            {
                transform.position = cameraTopDownPos;
                m_Camera.orthographicSize = (m_Camera.orthographicSize < desiredHalfHeight) ? m_Camera.orthographicSize + Time.deltaTime * zoomOutSpeed : desiredHalfHeight;
            }
            else if (!GameObject.Find("Maze").GetComponent<FindPathAStar>().isGameOver)
            {
                m_Camera.orthographicSize = desiredHalfHeight / 2;
                transform.position = betweenPlayersPos;
            }
        }

        else if (GameObject.Find("Maze").GetComponent<FindPathAStar>().player_1_wins || GameObject.Find("Maze").GetComponent<FindPathAStar>().player_2_wins)
        {
            ZoomWinner();
        }

    }

    void ZoomWinner()
    {
        // Hide boundaries
        GameObject[] walls;
        walls = GameObject.FindGameObjectsWithTag("Wall");
        foreach (GameObject wall in walls)
        {
            wall.gameObject.SetActive(false);
        }

        // Find winner
        GameObject winner;
        winner = (GameObject.Find("Maze").GetComponent<FindPathAStar>().player_1 == null) ? GameObject.Find("Maze").GetComponent<FindPathAStar>().player_2 : GameObject.Find("Maze").GetComponent<FindPathAStar>().player_1;

        // Hide level badge
        winner.transform.Find("Level 1").gameObject.SetActive(false);
        winner.transform.Find("Level 2").gameObject.SetActive(false);
        winner.transform.Find("Level 3").gameObject.SetActive(false);

        // Zoom 
        m_Camera.orthographicSize = (m_Camera.orthographicSize > zoomInSize) ? m_Camera.orthographicSize - Time.deltaTime : zoomInSize;
        transform.position = Vector3.Lerp(transform.position, winner.transform.position + new Vector3(0, 13, 0), Time.deltaTime * zoomInSpeed);
    }
}
