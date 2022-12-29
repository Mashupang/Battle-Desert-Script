using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MapLocation       
{
    public int x;
    public int z;

    public MapLocation(int _x, int _z)
    {
        x = _x;
        z = _z;
    }   

    public Vector2 ToVector()
    {
        return new Vector2(x, z);
    }

    public static MapLocation operator +(MapLocation a, MapLocation b)
       => new MapLocation(a.x + b.x, a.z + b.z);

    public override bool Equals(object obj)
    {
        if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            return false;
        else
            return x == ((MapLocation)obj).x && z == ((MapLocation)obj).z;
    }

    public override int GetHashCode()
    {
        return 0;
    }
}

public class Maze : MonoBehaviour
{
    public List<MapLocation> directions = new List<MapLocation>() {
                                            new MapLocation(1,0),
                                            new MapLocation(0,1),
                                            new MapLocation(-1,0),
                                            new MapLocation(0,-1) };
    public int width = 27; //x length
    public int depth = 17; //z length
    public int[,] map;
    public int scale = 1;

    private List<GameObject> wallsList = new List<GameObject>();
    private List<Vector3> wallsPosList = new List<Vector3>();
    public List<GameObject> prefabList = new List<GameObject>();
    private int[] indexTargetWall;
    private int[] indexInnerWall;
    public GameObject wallPrefeb1;
    public GameObject wallPrefeb2;

    // Start is called before the first frame update
    void Start()
    {
        prefabList.Add(wallPrefeb1);
        prefabList.Add(wallPrefeb2);
        InitialiseMap();
        //Generate();
        DrawMap();
        foreach (var gameObj in FindObjectsOfType(typeof(GameObject)) as GameObject[])
        {
            //if (gameObj.name == "Cube")
            //{
            //    wallsList.Add(gameObj);
            //}
            if (gameObj.name == "Wall1(Clone)" || gameObj.name == "Wall2(Clone)")
            {
                gameObj.tag = "Wall";
                wallsList.Add(gameObj);
            }
        }
        for(int i = 0;i < wallsList.Count; i++)
        {
            wallsPosList.Add(wallsList[i].transform.position);
        }
        TagTargetWall();
        TagInnerWall();
    }

    void TagTargetWall()
    {
        int targetWallIndex1 = wallsPosList.IndexOf(new Vector3(12, -0.5f, 8));
        int targetWallIndex2 = wallsPosList.IndexOf(new Vector3(14, -0.5f, 8));
        int targetWallIndex3 = wallsPosList.IndexOf(new Vector3(12, -0.5f, 9));
        int targetWallIndex4 = wallsPosList.IndexOf(new Vector3(13, -0.5f, 9));
        int targetWallIndex5 = wallsPosList.IndexOf(new Vector3(14, -0.5f, 9));
        int[] indexTargetWall = new int[] { targetWallIndex1, targetWallIndex2, targetWallIndex3, targetWallIndex4, targetWallIndex5 };
        for (int i = 0; i < indexTargetWall.Length; i++)
        {
            wallsList[indexTargetWall[i]].tag = "TargetWall";
            wallsList[indexTargetWall[i]].layer = LayerMask.NameToLayer("TargetWall");
        }
    }

    void TagInnerWall()
    {
        int innerWallIndex1 = wallsPosList.IndexOf(new Vector3(2, -0.5f, 8));
        int innerWallIndex2 = wallsPosList.IndexOf(new Vector3(3, -0.5f, 3));
        int innerWallIndex3 = wallsPosList.IndexOf(new Vector3(3, -0.5f, 4));
        int innerWallIndex4 = wallsPosList.IndexOf(new Vector3(3, -0.5f, 5));
        int innerWallIndex5 = wallsPosList.IndexOf(new Vector3(3, -0.5f, 6));
        int innerWallIndex6 = wallsPosList.IndexOf(new Vector3(3, -0.5f, 10));
        int innerWallIndex7 = wallsPosList.IndexOf(new Vector3(3, -0.5f, 11));
        int innerWallIndex8 = wallsPosList.IndexOf(new Vector3(3, -0.5f, 12));
        int innerWallIndex9 = wallsPosList.IndexOf(new Vector3(3, -0.5f, 13));
        int innerWallIndex10 = wallsPosList.IndexOf(new Vector3(4, -0.5f, 8));
        int innerWallIndex11 = wallsPosList.IndexOf(new Vector3(5, -0.5f, 3));
        int innerWallIndex12 = wallsPosList.IndexOf(new Vector3(5, -0.5f, 4));
        int innerWallIndex13 = wallsPosList.IndexOf(new Vector3(5, -0.5f, 5));
        int innerWallIndex14 = wallsPosList.IndexOf(new Vector3(5, -0.5f, 6));
        int innerWallIndex15 = wallsPosList.IndexOf(new Vector3(5, -0.5f, 8));
        int innerWallIndex16 = wallsPosList.IndexOf(new Vector3(5, -0.5f, 10));
        int innerWallIndex17 = wallsPosList.IndexOf(new Vector3(5, -0.5f, 11));
        int innerWallIndex18 = wallsPosList.IndexOf(new Vector3(5, -0.5f, 12));
        int innerWallIndex19 = wallsPosList.IndexOf(new Vector3(5, -0.5f, 13));
        int innerWallIndex20 = wallsPosList.IndexOf(new Vector3(7, -0.5f, 3));
        int innerWallIndex21 = wallsPosList.IndexOf(new Vector3(7, -0.5f, 4));
        int innerWallIndex22 = wallsPosList.IndexOf(new Vector3(7, -0.5f, 5));
        int innerWallIndex23 = wallsPosList.IndexOf(new Vector3(7, -0.5f, 6));
        int innerWallIndex24 = wallsPosList.IndexOf(new Vector3(7, -0.5f, 8));
        int innerWallIndex25 = wallsPosList.IndexOf(new Vector3(7, -0.5f, 10));
        int innerWallIndex26 = wallsPosList.IndexOf(new Vector3(7, -0.5f, 11));
        int innerWallIndex27 = wallsPosList.IndexOf(new Vector3(7, -0.5f, 12));
        int innerWallIndex28 = wallsPosList.IndexOf(new Vector3(7, -0.5f, 13));
        int innerWallIndex29 = wallsPosList.IndexOf(new Vector3(8, -0.5f, 8));
        int innerWallIndex30 = wallsPosList.IndexOf(new Vector3(9, -0.5f, 3));
        int innerWallIndex31 = wallsPosList.IndexOf(new Vector3(9, -0.5f, 4));
        int innerWallIndex32 = wallsPosList.IndexOf(new Vector3(9, -0.5f, 5));
        int innerWallIndex33 = wallsPosList.IndexOf(new Vector3(9, -0.5f, 6));
        int innerWallIndex34 = wallsPosList.IndexOf(new Vector3(9, -0.5f, 8));
        int innerWallIndex35 = wallsPosList.IndexOf(new Vector3(9, -0.5f, 10));
        int innerWallIndex36 = wallsPosList.IndexOf(new Vector3(9, -0.5f, 11));
        int innerWallIndex37 = wallsPosList.IndexOf(new Vector3(9, -0.5f, 12));
        int innerWallIndex38 = wallsPosList.IndexOf(new Vector3(9, -0.5f, 13));
        int innerWallIndex39 = wallsPosList.IndexOf(new Vector3(11, -0.5f, 3));
        int innerWallIndex40 = wallsPosList.IndexOf(new Vector3(11, -0.5f, 4));
        int innerWallIndex41 = wallsPosList.IndexOf(new Vector3(11, -0.5f, 5));
        int innerWallIndex42 = wallsPosList.IndexOf(new Vector3(11, -0.5f, 11));
        int innerWallIndex43 = wallsPosList.IndexOf(new Vector3(11, -0.5f, 12));
        int innerWallIndex44 = wallsPosList.IndexOf(new Vector3(11, -0.5f, 13));
        int innerWallIndex45 = wallsPosList.IndexOf(new Vector3(12, -0.5f, 5));
        int innerWallIndex46 = wallsPosList.IndexOf(new Vector3(12, -0.5f, 11));
        int innerWallIndex47 = wallsPosList.IndexOf(new Vector3(13, -0.5f, 3));
        int innerWallIndex48 = wallsPosList.IndexOf(new Vector3(13, -0.5f, 5));
        int innerWallIndex49 = wallsPosList.IndexOf(new Vector3(13, -0.5f, 11));
        int innerWallIndex50 = wallsPosList.IndexOf(new Vector3(13, -0.5f, 13));
        int innerWallIndex51 = wallsPosList.IndexOf(new Vector3(14, -0.5f, 5));
        int innerWallIndex52 = wallsPosList.IndexOf(new Vector3(14, -0.5f, 11));
        int innerWallIndex53 = wallsPosList.IndexOf(new Vector3(15, -0.5f, 3));
        int innerWallIndex54 = wallsPosList.IndexOf(new Vector3(15, -0.5f, 4));
        int innerWallIndex55 = wallsPosList.IndexOf(new Vector3(15, -0.5f, 5));
        int innerWallIndex56 = wallsPosList.IndexOf(new Vector3(15, -0.5f, 11));
        int innerWallIndex57 = wallsPosList.IndexOf(new Vector3(15, -0.5f, 12));
        int innerWallIndex58 = wallsPosList.IndexOf(new Vector3(15, -0.5f, 13));
        int innerWallIndex59 = wallsPosList.IndexOf(new Vector3(17, -0.5f, 3));
        int innerWallIndex60 = wallsPosList.IndexOf(new Vector3(17, -0.5f, 4));
        int innerWallIndex61 = wallsPosList.IndexOf(new Vector3(17, -0.5f, 5));
        int innerWallIndex62 = wallsPosList.IndexOf(new Vector3(17, -0.5f, 6));
        int innerWallIndex63 = wallsPosList.IndexOf(new Vector3(17, -0.5f, 8));
        int innerWallIndex64 = wallsPosList.IndexOf(new Vector3(17, -0.5f, 10));
        int innerWallIndex65 = wallsPosList.IndexOf(new Vector3(17, -0.5f, 11));
        int innerWallIndex66 = wallsPosList.IndexOf(new Vector3(17, -0.5f, 12));
        int innerWallIndex67 = wallsPosList.IndexOf(new Vector3(17, -0.5f, 13));
        int innerWallIndex68 = wallsPosList.IndexOf(new Vector3(18, -0.5f, 8));
        int innerWallIndex69 = wallsPosList.IndexOf(new Vector3(19, -0.5f, 3));
        int innerWallIndex70 = wallsPosList.IndexOf(new Vector3(19, -0.5f, 4));
        int innerWallIndex71 = wallsPosList.IndexOf(new Vector3(19, -0.5f, 5));
        int innerWallIndex72 = wallsPosList.IndexOf(new Vector3(19, -0.5f, 6));
        int innerWallIndex73 = wallsPosList.IndexOf(new Vector3(19, -0.5f, 8));
        int innerWallIndex74 = wallsPosList.IndexOf(new Vector3(19, -0.5f, 10));
        int innerWallIndex75 = wallsPosList.IndexOf(new Vector3(19, -0.5f, 11));
        int innerWallIndex76 = wallsPosList.IndexOf(new Vector3(19, -0.5f, 12));
        int innerWallIndex77 = wallsPosList.IndexOf(new Vector3(19, -0.5f, 13));
        int innerWallIndex78 = wallsPosList.IndexOf(new Vector3(21, -0.5f, 3));
        int innerWallIndex79 = wallsPosList.IndexOf(new Vector3(21, -0.5f, 4));
        int innerWallIndex80 = wallsPosList.IndexOf(new Vector3(21, -0.5f, 5));
        int innerWallIndex81 = wallsPosList.IndexOf(new Vector3(21, -0.5f, 6));
        int innerWallIndex82 = wallsPosList.IndexOf(new Vector3(21, -0.5f, 8));
        int innerWallIndex83 = wallsPosList.IndexOf(new Vector3(21, -0.5f, 10));
        int innerWallIndex84 = wallsPosList.IndexOf(new Vector3(21, -0.5f, 11));
        int innerWallIndex85 = wallsPosList.IndexOf(new Vector3(21, -0.5f, 12));
        int innerWallIndex86 = wallsPosList.IndexOf(new Vector3(21, -0.5f, 13));
        int innerWallIndex87 = wallsPosList.IndexOf(new Vector3(22, -0.5f, 8));
        int innerWallIndex88 = wallsPosList.IndexOf(new Vector3(23, -0.5f, 3));
        int innerWallIndex89 = wallsPosList.IndexOf(new Vector3(23, -0.5f, 4));
        int innerWallIndex90 = wallsPosList.IndexOf(new Vector3(23, -0.5f, 5));
        int innerWallIndex91 = wallsPosList.IndexOf(new Vector3(23, -0.5f, 6));
        int innerWallIndex92 = wallsPosList.IndexOf(new Vector3(23, -0.5f, 10));
        int innerWallIndex93 = wallsPosList.IndexOf(new Vector3(23, -0.5f, 11));
        int innerWallIndex94 = wallsPosList.IndexOf(new Vector3(23, -0.5f, 12));
        int innerWallIndex95 = wallsPosList.IndexOf(new Vector3(23, -0.5f, 13));
        int innerWallIndex96 = wallsPosList.IndexOf(new Vector3(24, -0.5f, 8));

        int[] indexInnerWall = new int[] {
            innerWallIndex1,
            innerWallIndex2,
            innerWallIndex3,
            innerWallIndex4,
            innerWallIndex5,
            innerWallIndex6,
            innerWallIndex7,
            innerWallIndex8,
            innerWallIndex9,
            innerWallIndex10,
            innerWallIndex11,
            innerWallIndex12,
            innerWallIndex13,
            innerWallIndex14,
            innerWallIndex15,
            innerWallIndex16,
            innerWallIndex17,
            innerWallIndex18,
            innerWallIndex19,
            innerWallIndex20,
            innerWallIndex21,
            innerWallIndex22,
            innerWallIndex23,
            innerWallIndex24,
            innerWallIndex25,
            innerWallIndex26,
            innerWallIndex27,
            innerWallIndex28,
            innerWallIndex29,
            innerWallIndex30,
            innerWallIndex31,
            innerWallIndex32,
            innerWallIndex33,
            innerWallIndex34,
            innerWallIndex35,
            innerWallIndex36,
            innerWallIndex37,
            innerWallIndex38,
            innerWallIndex39,
            innerWallIndex40,
            innerWallIndex41,
            innerWallIndex42,
            innerWallIndex43,
            innerWallIndex44,
            innerWallIndex45,
            innerWallIndex46,
            innerWallIndex47,
            innerWallIndex48,
            innerWallIndex49,
            innerWallIndex50,
            innerWallIndex51,
            innerWallIndex52,
            innerWallIndex53,
            innerWallIndex54,
            innerWallIndex55,
            innerWallIndex56,
            innerWallIndex57,
            innerWallIndex58,
            innerWallIndex59,
            innerWallIndex60,
            innerWallIndex61,
            innerWallIndex62,
            innerWallIndex63,
            innerWallIndex64,
            innerWallIndex65,
            innerWallIndex66,
            innerWallIndex67,
            innerWallIndex68,
            innerWallIndex69,
            innerWallIndex70,
            innerWallIndex71,
            innerWallIndex72,
            innerWallIndex73,
            innerWallIndex74,
            innerWallIndex75,
            innerWallIndex76,
            innerWallIndex77,
            innerWallIndex78,
            innerWallIndex79,
            innerWallIndex80,
            innerWallIndex81,
            innerWallIndex82,
            innerWallIndex83,
            innerWallIndex84,
            innerWallIndex85,
            innerWallIndex86,
            innerWallIndex87,
            innerWallIndex88,
            innerWallIndex89,
            innerWallIndex90,
            innerWallIndex91,
            innerWallIndex92,
            innerWallIndex93,
            innerWallIndex94,
            innerWallIndex95,
            innerWallIndex96
        };
        for (int i = 0; i < indexInnerWall.Length; i++)
        {
            wallsList[indexInnerWall[i]].tag = "InnerWall";
            wallsList[indexInnerWall[i]].layer = LayerMask.NameToLayer("InnerWall");
        }
    }

    void InitialiseMap()
    {
        map = new int[width,depth];
        for (int z = 0; z < depth; z++)
            for (int x = 0; x < width; x++)
            {
                map[x, z] = 0;     //1 = wall  0 = corridor
            }
        // Top walls
        map[0, 16] = 1;
        map[1, 16] = 1;
        map[2, 16] = 1;
        map[3, 16] = 1;
        map[4, 16] = 1;
        map[5, 16] = 1;
        map[6, 16] = 1;
        map[7, 16] = 1;
        map[8, 16] = 1;
        map[9, 16] = 1;
        map[10, 16] = 1;
        map[11, 16] = 1;
        map[12, 16] = 1;
        map[13, 16] = 1;
        map[14, 16] = 1;
        map[15, 16] = 1;
        map[16, 16] = 1;
        map[17, 16] = 1;
        map[18, 16] = 1;
        map[19, 16] = 1;
        map[20, 16] = 1;
        map[21, 16] = 1;
        map[22, 16] = 1;
        map[23, 16] = 1;
        map[24, 16] = 1;
        map[25, 16] = 1;
        map[26, 16] = 1;

        // Right walls
        map[26, 1] = 1;
        map[26, 2] = 1;
        map[26, 3] = 1;
        map[26, 4] = 1;
        map[26, 5] = 1;
        map[26, 6] = 1;
        map[26, 7] = 1;
        map[26, 8] = 1;
        map[26, 9] = 1;
        map[26, 10] = 1;
        map[26, 11] = 1;
        map[26, 12] = 1;
        map[26, 13] = 1;
        map[26, 14] = 1;
        map[26, 15] = 1;

        // Bottom walls
        map[0, 0] = 1;
        map[1, 0] = 1;
        map[2, 0] = 1;
        map[3, 0] = 1;
        map[4, 0] = 1;
        map[5, 0] = 1;
        map[6, 0] = 1;
        map[7, 0] = 1;
        map[8, 0] = 1;
        map[9, 0] = 1;
        map[10, 0] = 1;
        map[11, 0] = 1;
        map[12, 0] = 1;
        map[13, 0] = 1;
        map[14, 0] = 1;
        map[15, 0] = 1;
        map[16, 0] = 1;
        map[17, 0] = 1;
        map[18, 0] = 1;
        map[19, 0] = 1;
        map[20, 0] = 1;
        map[21, 0] = 1;
        map[22, 0] = 1;
        map[23, 0] = 1;
        map[24, 0] = 1;
        map[25, 0] = 1;
        map[26, 0] = 1;

        // Left walls
        map[0 ,1] = 1;
        map[0, 2] = 1;
        map[0, 3] = 1;
        map[0, 4] = 1;
        map[0, 5] = 1;
        map[0, 6] = 1;
        map[0, 7] = 1;
        map[0, 8] = 1;
        map[0, 9] = 1;
        map[0, 10] = 1;
        map[0, 11] = 1;
        map[0, 12] = 1;
        map[0, 13] = 1;
        map[0, 14] = 1;
        map[0, 15] = 1;       

        // Target walls
        map[12, 8] = 1;
        map[14, 8] = 1;
        map[12, 9] = 1;        
        map[13, 9] = 1;
        map[14, 9] = 1;

        // Inner walls
        map[2, 8] = 1;
        map[3, 3] = 1;
        map[3, 4] = 1;
        map[3, 5] = 1;
        map[3, 6] = 1;
        map[3, 10] = 1;
        map[3, 11] = 1;
        map[3, 12] = 1;
        map[3, 13] = 1;
        map[4, 8] = 1;
        map[5, 3] = 1;
        map[5, 4] = 1;
        map[5, 5] = 1;
        map[5, 6] = 1;
        map[5, 8] = 1;
        map[5, 10] = 1;
        map[5, 11] = 1;
        map[5, 12] = 1;
        map[5, 13] = 1;
        map[7, 3] = 1;
        map[7, 4] = 1;
        map[7, 5] = 1;
        map[7, 6] = 1;
        map[7, 8] = 1;
        map[7, 10] = 1;
        map[7, 11] = 1;
        map[7, 12] = 1;
        map[7, 13] = 1;
        map[8, 8] = 1;
        map[9, 3] = 1;
        map[9, 4] = 1;
        map[9, 5] = 1;
        map[9, 6] = 1;
        map[9, 8] = 1;
        map[9, 10] = 1;
        map[9, 11] = 1;
        map[9, 12] = 1;
        map[9, 13] = 1;
        map[11, 3] = 1;
        map[11, 4] = 1;
        map[11, 5] = 1;
        map[11, 11] = 1;
        map[11, 12] = 1;
        map[11, 13] = 1;
        map[12, 5] = 1;
        map[12, 11] = 1;
        map[13, 3] = 1;
        map[13, 5] = 1;
        map[13, 11] = 1;
        map[13, 13] = 1;
        map[14, 5] = 1;
        map[14, 11] = 1;
        map[15, 3] = 1;
        map[15, 4] = 1;
        map[15, 5] = 1;
        map[15, 11] = 1;
        map[15, 12] = 1;
        map[15, 13] = 1;
        map[17, 3] = 1;
        map[17, 4] = 1;
        map[17, 5] = 1;
        map[17, 6] = 1;
        map[17, 8] = 1;
        map[17, 10] = 1;
        map[17, 11] = 1;
        map[17, 12] = 1;
        map[17, 13] = 1;
        map[18, 8] = 1;
        map[19, 3] = 1;
        map[19, 4] = 1;
        map[19, 5] = 1;
        map[19, 6] = 1;
        map[19, 8] = 1;
        map[19, 10] = 1;
        map[19, 11] = 1;
        map[19, 12] = 1;
        map[19, 13] = 1;
        map[21, 3] = 1;
        map[21, 4] = 1;
        map[21, 5] = 1;
        map[21, 6] = 1;
        map[21, 8] = 1;
        map[21, 10] = 1;
        map[21, 11] = 1;
        map[21, 12] = 1;
        map[21, 13] = 1;
        map[22, 8] = 1;
        map[23, 3] = 1;
        map[23, 4] = 1;
        map[23, 5] = 1;
        map[23, 6] = 1;
        map[23, 10] = 1;
        map[23, 11] = 1;
        map[23, 12] = 1;
        map[23, 13] = 1;
        map[24, 8] = 1;

    }

    public virtual void Generate()
    {
        for (int z = 0; z < depth; z++)
            for (int x = 0; x < width; x++)
            {
               if(Random.Range(0,100) < 50)
                 map[x, z] = 0;     //1 = wall  0 = corridor
            }
        
    }

    void DrawMap()
    {
        for (int z = 0; z < depth; z++)
        {
            for (int x = 0; x < width; x++)
            {
                if (map[x, z] == 1)
                {
                    //Vector3 pos = new Vector3(x * scale, 0, z * scale);
                    Vector3 pos = new Vector3(x * scale, 0, z * scale);
                    //GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    int prefabIndex = Random.Range(0, 2);

                    Instantiate(prefabList[prefabIndex], new Vector3(x * scale, -0.5f, z * scale), transform.rotation * Quaternion.Euler(0f, 0f, 0f));
                    //wall.tag = "Wall";
                    //wall.transform.localScale = new Vector3(scale, scale, scale);
                    //wall.transform.position = pos;

                }
            }
        }
        
    }

    public int CountSquareNeighbours(int x, int z)
    {
        int count = 0;
        if (x <= 0 || x >= width - 1 || z <= 0 || z >= depth - 1) return 5;
        if (map[x - 1, z] == 0) count++;
        if (map[x + 1, z] == 0) count++;
        if (map[x, z + 1] == 0) count++;
        if (map[x, z - 1] == 0) count++;
        return count;
    }

    public int CountDiagonalNeighbours(int x, int z)
    {
        int count = 0;
        if (x <= 0 || x >= width - 1 || z <= 0 || z >= depth - 1) return 5;
        if (map[x - 1, z - 1] == 0) count++;
        if (map[x + 1, z + 1] == 0) count++;
        if (map[x - 1, z + 1] == 0) count++;
        if (map[x + 1, z - 1] == 0) count++;
        return count;
    }

    public int CountAllNeighbours(int x, int z)
    {
        return CountSquareNeighbours(x,z) + CountDiagonalNeighbours(x,z);
    }


    void CheckWalls()
    {        
        for (int i = 0; i < wallsList.Count; i++)
        {
            if (wallsList[i] == null)
            {
                map[(int)wallsPosList[i].x, (int)wallsPosList[i].z] = 0;
            }
        }

    }

    void Update()
    {
        //CheckWalls();
    }


    public void MarkWallZero(Vector3 wallPos)
    {
        map[(int)wallPos.x, (int)wallPos.z] = 0;
    }
}
