using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public GameObject tileg, tilegr1, tilegr2, tilegr21, tilegr22, tilegr3, tilegr4; //grass road objects
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
        for (int i = 0; i < 4; i++)
        {
            map[mapSize / 2][mapSize / 2].changeSide(1, i);
            map[(mapSize / 2) + snakeDirections[i][0]][(mapSize / 2) + snakeDirections[i][1]].changeSide(1, ((i+2)%4));
            int roadLength = Random.Range(60, 100);
            Debug.Log(roadLength);
            int[] starter = new int[] { (mapSize / 2) + snakeDirections[i][0], (mapSize / 2) + snakeDirections[i][1] };
            snakeNode(starter, 1, ((i + 2) % 4), 0, 0, roadLength);
        }
        for(int i = 0; i < mapSize; i++) // North = z, East = x, all facing north
        {
            for(int j = 0; j < mapSize; j++)
            {
                TileObject curr = map[i][j];
                GameObject currObj;
                if(curr.hasCity == true)
                {
                    // put this stuff in when ya get there
                    currObj = GameObject.Instantiate(tileg);
                }
                else 
                {
                    if(curr.countType(1) == 1)
                    {
                        currObj = GameObject.Instantiate(tilegr1);
                        int[] aroundInfo = checkTilesAround(new int[] { i, j });
                        int multiplier = 0;
                        foreach (int x in aroundInfo)
                            if (x != 1)
                                multiplier++;
                        currObj.transform.Rotate(0.0f, 90.0f * multiplier, 0.0f, Space.Self);
                    }
                    else if (curr.countType(1) == 2)
                    {
                        currObj = GameObject.Instantiate(tilegr2);
                    }
                    else if (curr.countType(1) == 3)
                    {
                        currObj = GameObject.Instantiate(tilegr3);
                    }
                    else if (curr.countType(1) == 4)
                    {
                        currObj = GameObject.Instantiate(tilegr4);
                    }
                    else
                    {
                        currObj = GameObject.Instantiate(tileg);
                    }
                }
                currObj.transform.position = new Vector3(20 * (i - mapSize / 2), -3.6f, 20 * (j - mapSize / 2));
            }
        }
        
    }

    public bool snakeNode(int[] currCoords, int type, int currDir, int lastTurn, int currIt, int itLimit) //01234 empty,road,path,rail,river 0123 NESW, 123 LSR
    {
        if ((currIt >= itLimit) == false)
        {
            int newTurn = Random.Range(1, 4);
            if((newTurn == lastTurn && lastTurn != 1) || newTurn > 3)
                newTurn = 2;
            int newDir = (currDir + newTurn) % 4;
            int[] newCoords = new int[] { currCoords[0] + snakeDirections[newDir][0], currCoords[1] + snakeDirections[newDir][1] };
            map[currCoords[0]][currCoords[1]].changeSide(type,newDir);
            map[newCoords[0]][newCoords[1]].changeSide(type, (newDir+2)%4);
            return snakeNode(newCoords, type, newDir, newTurn, (currIt+1), itLimit);
        }
        else
        {
            Debug.Log("Completed snake");
            return true;
        }
    }

    public int[] checkTilesAround(int[] coords) // 0123 ESWN
    {
        int[] tiles = new int[4];
        for (int i = 0; i < 4; i++)
        {
            TileObject currTile = map[coords[0] + snakeDirections[i][0]][coords[1] + snakeDirections[i][1]];
            if (i == 0)
                tiles[i] = currTile.South.stru;
            else if (i == 1)
                tiles[i] = currTile.West.stru;
            else if (i == 2)
                tiles[i] = currTile.North.stru;
            else
                tiles[i] = currTile.East.stru;
            
        }
        return tiles;
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
    public bool hasCity { get; set; }
    public TileObject()
    {
        North = new TileSide();
        East = new TileSide();
        South = new TileSide();
        West = new TileSide();
        hasCity = false;
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
        if (North.stru == type || East.stru == type || South.stru == type || West.stru == type)
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
}

public class TileSide // 01234 empty,road,path,rail,river 
{
    public int stru { get; set; }

    public TileSide()
    {
        stru = 0;
    }

}
