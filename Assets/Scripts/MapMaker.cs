using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 * New road gen 
 * snake around roads, pick 3 and 4 sided, search around always turning right. if intersects with origin make everything inside a city (keep track of turns and use farthest and innermost coords)
*/
public class MapMaker : MonoBehaviour
{
    public int mapSize = 101;
    public List<List<TileObject>> map = new List<List<TileObject>>();
    public List<int[]> snakeDirections = new List<int[]>() {
        new int[] {-1,0},
        new int[] {0,1},
        new int[] {1,0},
        new int[] {0,-1}
    };
    public List<int[]> rangeDirections = new List<int[]>() {
        new int[] {-1,0},
        new int[] {-1,1},
        new int[] {0,1},
        new int[] {1,1},
        new int[] {1,0},
        new int[] {1,-1},
        new int[] {0,-1},
        new int[] {-1,-1},
    };
    public GameObject tileg, tilegr1, tilegr2, tilegr21, tilegr22, tilegr3, tilegr4, tilegR1, tilegR2, tilegR21; //grass road objects
    public GameObject tilec, tilecr1, tilecr2, tilecr21, tilecr22, tilecr3, tilecr4; //city road objects
    public GameObject tilef, tilef1, tilef2, tilef3, tilefR2, tilefR21; //forest objects
    public GameObject b111, b112, b113, b114, b115, b116, b117; //buildings
    public GameObject single, horde3, horde5, horde7; // hordes
    // Start is called before the first frame update
    void Start()
    {
        for (int x = 0; x < mapSize; x++)
        {
            List<TileObject> temp = new List<TileObject>();
            for (int y = 0; y < mapSize; y++)
            {
                temp.Add(new TileObject());
            }
            map.Add(temp);
        }
        // sets middle tile as fourway road
        int totalRoad = 0;
        for (int i = 0; i < 4; i++)
        {
            map[mapSize / 2][mapSize / 2].changeSide(1, i);
            map[(mapSize / 2) + snakeDirections[i][0]][(mapSize / 2) + snakeDirections[i][1]].changeSide(1, ((i+2)%4));
            int roadLength = UnityEngine.Random.Range(100, 250);
            totalRoad += roadLength;
            Debug.Log(roadLength);
            int[] starter = new int[] { (mapSize / 2) + snakeDirections[i][0], (mapSize / 2) + snakeDirections[i][1] };
            snakeNode(starter, 1, ((i + 2) % 4), 1, 0, roadLength);
        }

        for (int i = 0; i < mapSize; i++) // adding areas
        {
            for (int j = 0; j < mapSize; j++)
            {
                /*
                if (map[i][j].countType(1) > 2 && map[i][j].tileType == 0)
                {
                    List<int[]> cm = new List<int[]>();
                    cm.Add(new int[] { i, j });
                    int val = 1;
                    if (map[i][j].East.stru != 1)
                        val = 2;
                    cm = cityMaker(new int[] { i, j }, val, cm, 0, totalRoad);
                    if(cm.Count > 0)
                    {
                        Debug.Log("cm");
                        int sX = mapSize - 1, sY = mapSize-1;
                        int lX = 0, lY = 0;
                        foreach (int[] u in cm)
                        {
                            if (u[0] < sX)
                                sX = u[0];
                            else if (u[0] > lX)
                                lX = u[0];
                            if (u[1] < sY)
                                sY = u[1];
                            else if (u[1] > lY)
                                lY = u[1];
                        }
                        for(int x = sX; x <= lX; x++)
                        {
                            for(int y = sY; y <= lY; y++)
                            {
                                map[x][y].tileType = 1;
                            }
                        }
                    }
                }
                */
                int totalRoads = 0;
                for(int x = 0; x < 8; x++)
                {
                    totalRoads += checkRange(new int[] { i,j }, 1, x, 0, 15, 0,false);
                }
                if(totalRoads > 5)
                {
                    map[i][j].tileType = 1;
                }
                else if(totalRoads < 2 && map[i][j].countType(1) < 1)
                {
                    map[i][j].tileType = 2;
                }
                
            }
        }

        for (int i = 0; i < mapSize; i++) // North = z, East = x, all facing north
        {
            for(int j = 0; j < mapSize; j++)
            {
                TileObject curr = map[i][j];
                GameObject currObj;
                int[] aroundInfo = curr.getTilesTypes();
                if (curr.tileType == 2)
                {
                    int now = UnityEngine.Random.Range(1, 11);
                    if (now == 1)
                        currObj = GameObject.Instantiate(tilef1);
                    else if (now == 2)
                        currObj = GameObject.Instantiate(tilef);
                    else if (now > 2 && now < 7)
                        currObj = GameObject.Instantiate(tilef2);
                    else
                        currObj = GameObject.Instantiate(tilef3);
                    zombieSpawn(new int[] { i, j });
                }
                else 
                {
                    if(curr.countType(1) == 1)
                    {
                        if (curr.tileType == 1)
                            currObj = GameObject.Instantiate(tilecr1);
                        else
                            currObj = GameObject.Instantiate(tilegr1);

                        int multiplier = -1;
                        foreach (int x in aroundInfo)
                        {
                            if (x != 1)
                                multiplier++;
                            else
                                break;
                        }
                        Debug.Log(multiplier);
                        currObj.transform.Rotate(0.0f, 90.0f * multiplier, 0.0f);
                        zombieSpawn(new int[] { i, j });
                    }
                    else if (curr.countType(1) == 2)
                    {
                        int countBetween = 0;
                        int posFirst = -1;
                        for(int k = 0; k < aroundInfo.Length; k++)
                        {
                            if (aroundInfo[k] == 1)
                            {
                                if (posFirst == -1)
                                    posFirst = k;
                                else
                                    break;
                            }
                            else
                            {
                                if(posFirst != -1)
                                    countBetween++;
                            }
                        }
                        posFirst--;
                        if (countBetween == 0)
                        {
                            if (curr.tileType == 1)
                                currObj = GameObject.Instantiate(tilecr22);
                            else
                                currObj = GameObject.Instantiate(tilegr22);
                            currObj.transform.Rotate(0.0f, 90.0f * posFirst, 0.0f);
                            zombieSpawn(new int[] { i, j });
                        }
                        else if (countBetween == 2)
                        {
                            if (curr.tileType == 1)
                                currObj = GameObject.Instantiate(tilecr22);
                            else
                                currObj = GameObject.Instantiate(tilegr22);
                            currObj.transform.Rotate(0.0f, 90.0f * 2, 0.0f);
                            zombieSpawn(new int[] { i, j });
                        }
                        else
                        {
                            bool ifCross = false;
                            int turnCount = 0;
                            for (int x = 0; x < 4; x++)
                            {
                                if ((i + snakeDirections[x][0]) < 0 || (i + snakeDirections[x][0]) >= mapSize || (j + snakeDirections[x][1]) < 0 || (j + snakeDirections[x][1]) >= mapSize)
                                {
                                    break;
                                }
                                else
                                {
                                    if (map[(i + snakeDirections[x][0])][(j + snakeDirections[x][1])].contains(0) == false)
                                    {
                                        ifCross = true;
                                        turnCount = (x + 2) % 4 - 1;
                                        break;
                                    }
                                }
                            }
                            if (ifCross)
                            {
                                if (curr.tileType == 1)
                                    currObj = GameObject.Instantiate(tilecr21);
                                else
                                    currObj = GameObject.Instantiate(tilegr21);
                                currObj.transform.Rotate(0.0f, 90.0f * turnCount, 0.0f);
                            }
                            else
                            {
                                if (curr.tileType == 1)
                                    currObj = GameObject.Instantiate(tilecr2);
                                else
                                    currObj = GameObject.Instantiate(tilegr2);
                                currObj.transform.Rotate(0.0f, 90.0f * posFirst, 0.0f);
                            }
                            zombieSpawn(new int[] { i, j });
                        }
                    }
                    else if (curr.countType(1) == 3)
                    {
                        if (curr.tileType == 1)
                            currObj = GameObject.Instantiate(tilecr3);
                        else
                            currObj = GameObject.Instantiate(tilegr3);
                        int pos = Array.IndexOf(aroundInfo, 0);
                        currObj.transform.Rotate(0.0f, 90.0f * pos, 0.0f);
                        zombieSpawn(new int[] { i, j });
                    }
                    else if (curr.countType(1) == 4)
                    {
                        if (curr.tileType == 1)
                            currObj = GameObject.Instantiate(tilecr4);
                        else
                            currObj = GameObject.Instantiate(tilegr4);
                        zombieSpawn(new int[] { i, j });
                    }
                    else
                    {
                        if (curr.tileType == 1)
                        {
                            int buildingType = UnityEngine.Random.Range(0, 5);
                            GameObject[] building = new GameObject[] { b111, b112, b113, b114, b115 };
                            int ifNearRoad = -1;
                            for (int x = 0; x < 8; x += 2)
                            {
                                if (checkRange(new int[] { i, j }, 1, x, 0, 2, 0, false) > 0)
                                {
                                    ifNearRoad = x / 2;
                                    break;
                                }
                            }
                            if (ifNearRoad > -1)
                            {
                                currObj = GameObject.Instantiate(building[buildingType]);
                                currObj.transform.Rotate(0.0f, 90.0f * ifNearRoad - 90.0f, 0.0f);
                            }
                            else
                            {
                                currObj = GameObject.Instantiate(tilec);
                                zombieSpawn(new int[] { i, j });
                            }
                        }
                        else
                        {
                            currObj = GameObject.Instantiate(tileg);
                            zombieSpawn(new int[] { i, j });
                        }
                    }
                }
                currObj.transform.position = new Vector3(20 * (i - (mapSize / 2)), -3.6f, 20 * (j - (mapSize / 2)));
                
            }
            
        }
        
    }

    public void zombieSpawn(int[] currCoords)
    {
        if (currCoords[0] != mapSize / 2 && currCoords[1] != mapSize / 2)
        {
            int i = currCoords[0];
            int j = currCoords[1];
            bool road = map[currCoords[0]][currCoords[1]].contains(1);
            int tT = map[currCoords[0]][currCoords[1]].tileType;
            int based = 800;
            if (road)
                based = based / 5;
            if (tT == 1)
                based = based / 2;
            else if (tT == 2)
                based = based * 2;

            int zombieRNG = UnityEngine.Random.Range(0, based + 1);
            if (zombieRNG < 1)
            {
                GameObject currObj = GameObject.Instantiate(horde7);
                currObj.transform.position = new Vector3(20 * (i - (mapSize / 2)), 1, 20 * (j - (mapSize / 2)));
            }
            else if (zombieRNG < 4)
            {
                GameObject currObj = GameObject.Instantiate(horde5);
                currObj.transform.position = new Vector3(20 * (i - (mapSize / 2)), 1, 20 * (j - (mapSize / 2)));
            }
            else if (zombieRNG < 10)
            {
                GameObject currObj = GameObject.Instantiate(horde3);
                currObj.transform.position = new Vector3(20 * (i - (mapSize / 2)), 1, 20 * (j - (mapSize / 2)));
            }
            else if (zombieRNG < 25)
            {
                GameObject currObj = GameObject.Instantiate(single);
                currObj.transform.position = new Vector3(20 * (i - (mapSize / 2)), 1, 20 * (j - (mapSize / 2)));
            }
        }
    }

    public bool snakeNode(int[] currCoords, int type, int currDir, int lastTurn, int currIt, int itLimit) //01234 empty,road,path,rail,river 0123 NESW, 123 LSR
    {
        if (currIt >= itLimit)
        {
            Debug.Log("Completed snake");
            return true;
        }
        else
        {
            int newTurn = UnityEngine.Random.Range(1, 20);
            if ((newTurn == lastTurn && lastTurn != 1) || newTurn > 3)
                newTurn = 2;
            int newDir = (currDir + newTurn) % 4;
            int[] newCoords = new int[] { currCoords[0] + snakeDirections[newDir][0], currCoords[1] + snakeDirections[newDir][1] };
            if(newCoords[0] < 0 || newCoords[0] >= mapSize || newCoords[1] < 0 || newCoords[1] >= mapSize)
            {
                Debug.Log("Completed snake by edge");
                return true;
            }
            map[currCoords[0]][currCoords[1]].changeSide(type, newDir);
            map[newCoords[0]][newCoords[1]].changeSide(type, ((newDir + 2) % 4));
            int newIt = currIt + 1;
            Debug.Log("Tile "+currCoords[0] + " " + currCoords[1] + " bearing " + newDir);
            return snakeNode(newCoords, type, ((newDir + 2) % 4), newTurn, newIt, itLimit);
        }
    }

    public List<int[]> cityMaker(int[] currCoords, int currDir, List<int[]> allTurns, int iter, int totalRoad)
    {
        if (iter < totalRoad)
        {
            TileObject curr = map[currCoords[0]][currCoords[1]];
            int[] around = curr.getTilesTypes();
            int newDir = currDir;
            for (int x = 0; x < 3; x++)
            {
                int xR = ((x + currDir + 1) % 4);
                if (around[xR] == 1)
                {
                    newDir = xR;
                    break;
                }
            }
            if (newDir == currDir)
                return new List<int[]> { };
            int[] newCoords = new int[] { currCoords[0] + snakeDirections[newDir][0], currCoords[1] + snakeDirections[newDir][1] };
            Debug.Log(newCoords[0] + " " + newCoords[1] + " " + iter);
            if (allTurns.Contains(newCoords))
                return allTurns;
            allTurns.Add(newCoords);
            return cityMaker(newCoords, (newDir + 2) % 4, allTurns, iter+1, totalRoad);
        }
        return new List<int[]> { };
    }

    public int checkRange(int[] currCoords, int type, int dir, int currIt, int length, int total, bool twice)
    {
        int c = 1 + (dir % 2);
        if(currIt >= length)
        {
            return total;
        }
        else if(currCoords[0] < 0 || currCoords[0] >= mapSize || currCoords[1] < 0 || currCoords[1] >= mapSize)
        {
            return total;
        }
        else if(map[currCoords[0]][currCoords[1]].contains(type))
        {
            if(twice)
                return checkRange(new int[] { currCoords[0] + (rangeDirections[dir][0] / c), currCoords[1] + (rangeDirections[dir][1] / c) }, type, dir, currIt + 1, length, total, true);
            return checkRange(new int[] { currCoords[0] + (rangeDirections[dir][0] / c), currCoords[1] + (rangeDirections[dir][1] / c) }, type, dir, currIt + 1, length, total+1, true);
        }
        else
        {
            return checkRange(new int[] { currCoords[0] + (rangeDirections[dir][0] / c), currCoords[1] + (rangeDirections[dir][1] / c) }, type, dir, currIt + 1, length, total, false);
        }
    }



    //creates a map 
    /*
        Areas on Map
        - Grassy area
        - Roads 
        - Cities
        - Forest
        - Railway
        - River
    */
    // Update is called once per frame
    void Update()
    {
        
    }
}

public class TileObject
{

    public TileSide North { get; set; }
    public TileSide East { get; set; }
    public TileSide South { get; set; }
    public TileSide West { get; set; }
    public int tileType { get; set; } // 012 grass/city/forest
    public TileObject()
    {
        North = new TileSide();
        East = new TileSide();
        South = new TileSide();
        West = new TileSide();
        tileType = 0;
    }
    public void changeSide(int type, int side) //0123 NESW 01234 empty,road,path,rail,river 
    {
        if (side == 0)
        {
            North.stru = type;
        }
        else if (side == 1)
        {
            East.stru = type;
        }
        else if (side == 2)
        {
            South.stru = type;
        }
        else if (side == 3)
        {
            West.stru = type;
        }
    }
    public bool contains(int type)
    {
        if (Array.Exists(getTilesTypes(), element => element == type))
            return true;
        return false;
    }
    public int countType(int type)
    {
        int total = 0;
        bool[] totals = new bool[] { North.stru == type, East.stru == type, South.stru == type, West.stru == type };
        foreach(bool x in totals) { if(x) { total++; } }
        return total;
    }
    public int[] getTilesTypes()
    {
        return new int[] { North.stru, East.stru, South.stru, West.stru };
    }
}

public class TileSide // 01234 empty,road,path,rail,river 
{
    public int stru { get; set; }

    public TileSide()
    {
        stru = 0;
    }

}
