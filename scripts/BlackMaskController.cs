using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class BlackMaskController : MonoBehaviour
{
    private List<GameObject> masksList = new List<GameObject>();
    private List<Vector3> masksPosList = new List<Vector3>();
    private int mapWidth = 27; // map x length
    private int mapDepth = 17; // map z length
    private float maskHeight = 2f; // mask y height
    private Vector3 player1Position = new Vector3(12, 0, 7);
    private Vector3 player2Position = new Vector3(14, 0, 7);
    private Color maskFull = new Color(0f, 0f, 0f, 1f);
    private Color maskHalf = new Color(0f, 0f, 0f, 0.5f);
    private Color maskZero = new Color(0f, 0f, 0f, 0f);
    private int revealSquareMaskHalfSideLength = 3;
    private int stage = 0;
    private string[,] maskLabel;    
    private bool player1moving;
    private bool player2moving;
    //public GameObject maskLabelCanvas;


    // Start is called before the first frame update
    void Start()
    {     
        DrawMask(maskZero);
        foreach (var gameObj in FindObjectsOfType(typeof(GameObject)) as GameObject[])
        {
            if (gameObj.tag == "Mask")
            {
                masksList.Add(gameObj);
            }
        }

        for (int i = 0; i < masksList.Count; i++)
        {
            masksPosList.Add(masksList[i].transform.position);
        }

        maskLabel = new string[mapWidth, mapDepth];
        for (int z = 0; z < mapDepth; z++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                maskLabel[x, z] = "0";


                //DisplayMaskLabel(x + "," + z, "0", new Vector3(10 * x, 10 * z, 0));

            }
        }

    }

    /*
    void DisplayMaskLabel(string objName, string objText, Vector3 textPos)
    {
        Font arial;
        arial = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
        GameObject textGO = new GameObject();
        textGO.name = objName;
        textGO.transform.parent = maskLabelCanvas.transform;
        textGO.transform.eulerAngles = new Vector3(90, 0, 0);
        textGO.AddComponent<Text>();
        Text text = textGO.GetComponent<Text>();
        text.font = arial;
        text.text = objText;
        text.fontSize = 48;
        text.alignment = TextAnchor.MiddleCenter;
        RectTransform rectTransform;
        rectTransform = text.GetComponent<RectTransform>();
        rectTransform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        rectTransform.localPosition = textPos;
    }
    */

    void DrawMask(Color maskColor)
    {
        float scale = 0.1f;
        for (int z = 0; z < mapDepth; z++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                Vector3 pos = new Vector3(x, maskHeight, z);
                GameObject mask = GameObject.CreatePrimitive(PrimitiveType.Plane);
                mask.GetComponent<Renderer>().material.color = maskColor;
                mask.name = "Mask(" + x + ", " + maskHeight + ", " + z + ")";
                mask.tag = "Mask";
                mask.layer = LayerMask.NameToLayer("Mask");
                mask.transform.localScale = new Vector3(scale, scale, scale);
                mask.transform.position = pos;

                Material mat = mask.GetComponent<Renderer>().sharedMaterial;

                mat.SetFloat("_Mode", 2);
                mat.SetFloat("_Metallic", 1f);
                mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                mat.SetInt("_ZWrite", 0);
                mat.DisableKeyword("_ALPHATEST_ON");
                mat.EnableKeyword("_ALPHABLEND_ON");
                mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");                
                mat.renderQueue = 3000;              

            }
        }

    }

    public void PlayersVisibility(Vector3 playerPos, Vector3 movePointPos, string inputID)
    {

        if (playerPos == movePointPos)
        {
            if(inputID == "1")
            {
                CoverMask(player1Position, 2); // cover previous mask
                player1Position = movePointPos; // update last position
                
            }
            else if (inputID == "2")
            {
                CoverMask(player2Position, 1); // cover previous mask
                player2Position = movePointPos; // update last position
                
            }
        }
        ScoutMap(movePointPos, inputID);    
    }

    void ScoutMap(Vector3 movePointPos, string inputID)
    {
        if(stage != 0)
        {
            EditMask(movePointPos, maskZero, inputID);

        }       

    }



    public void CoverMask(Vector3 movePointPos, int otherPlayerID = 0)
    {
        if(stage == 1)
        {
            EditMask(movePointPos, maskHalf, "0", otherPlayerID);

        }
        else if (stage == 2)
        {
            EditMask(movePointPos, maskFull, "0", otherPlayerID);

        }
    }

    void EditMask(Vector3 movePointPos, Color maskColor, string inputID, int otherPlayerID = 0)
    {
        var intInputID = Convert.ToInt32(inputID);

        for (int i = (int)movePointPos.x - revealSquareMaskHalfSideLength; i < (int)movePointPos.x + revealSquareMaskHalfSideLength + 1; i++)
        {

            if(i < 0 || i > (mapWidth - 1))
            {
                continue;
            }
            for (int j = (int)movePointPos.z - revealSquareMaskHalfSideLength; j < (int)movePointPos.z + revealSquareMaskHalfSideLength + 1; j++)
            {
                if (j < 0 || j > (mapDepth - 1))
                {
                    continue;
                }
                var intMaskLabel = Convert.ToInt32(maskLabel[i, j]);
                if (intMaskLabel + intInputID == 3 && inputID != "0")
                {
                    continue;
                }
                else if (inputID == "0")
                {
                    if (intMaskLabel == otherPlayerID)
                    {
                        continue;
                    }
                }
                Vector3 matchMaskPos = new Vector3(i, maskHeight, j);
                int IndexOfMask = masksPosList.IndexOf(matchMaskPos);
                masksList[IndexOfMask].GetComponent<Renderer>().material.color = maskColor;
                maskLabel[i, j] = inputID;


                /*  
                string objName = i.ToString() + "," + j.ToString();
                Text txt = maskLabelCanvas.transform.Find(objName).GetComponent<Text>();
                txt.text = inputID;
                */
            }
        }
    }

    public void NextStage()
    {
        stage++;
        for (int i = 0; i < masksList.Count; i++)
        {
            if (stage == 1)
            {
                masksList[i].GetComponent<Renderer>().material.color = maskHalf;
            }
            else if (stage == 2)
            {
                masksList[i].GetComponent<Renderer>().material.color = maskFull;
            }
        }
    }
}
