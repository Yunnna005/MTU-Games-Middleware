using Unity.VisualScripting;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices.WindowsRuntime;

public class DungeonGeneration : MonoBehaviour
{
    public class Cell
    {
        public bool visited = false;
        public bool[] status = new bool[4]; // Up, Down, Left, Right
    }

    [System.Serializable]
    public class Rule
    {
        public GameObject room;
        public Vector2Int minPos;
        public Vector2Int maxPos;

        public bool obligatory;

        public int Probability(int x, int y)
        {
            //0 - cannot spavn , 1 - can spawn, 2 - must spawn
            if(x >= minPos.x && x <= maxPos.x && y >= minPos.y && y <= maxPos.y)
            {
                return obligatory ? 2 : 1;
            }


            return 0;
        }
    }

    public Vector2Int size;
    public int startPos = 0;
    public Rule[] rooms;
    public Vector2 offset;

    List<Cell> board;

    void Start()
    {
        MazeGenerator();
    }

    void Update()
    {
        
    }

    void GenerateDungeon()
    {
        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {

                Cell currentCell = board[i + j * size.x];
                if (currentCell.visited)
                {
                    int randomRoom = -1;
                    List<int> possibleRooms = new List<int>();

                    for (int r = 0; r < rooms.Length; r++)
                    {
                        int prob = rooms[r].Probability(i, j);
                        if (prob == 2)
                        {
                            randomRoom = r;
                            break;
                        }
                        else if (prob == 1)
                        {
                            possibleRooms.Add(r);
                        }
                    }

                    if (randomRoom == -1)
                    {
                        if (possibleRooms.Count > 0)
                        {
                            randomRoom = possibleRooms[Random.Range(0, possibleRooms.Count)];
                        }
                        else
                        {
                            randomRoom = 0;
                        }
                    }
                    var newRoom = Instantiate(rooms[randomRoom].room, new Vector3(i * offset.x, 0, -j * offset.y), Quaternion.identity, transform).GetComponent<RoomBahaviour>();
                    newRoom.RoomUpdate(board[Mathf.FloorToInt(i + j * size.x)].status);

                    newRoom.name += " " + i + "-" + j;
                }
            }
        }
    }

    public void MazeGenerator()
    {
        board = new List<Cell>();

        for (int i = 0; i < size.x * size.y; i++)
        {
            board.Add(new Cell());
        }

        int currentCell = startPos;

        Stack<int> path = new Stack<int>();

        int k = 0;

        while (k<1000)
        {
            k++;

            board[currentCell].visited = true;

            if (currentCell == board.Count -1 )
            {
                break;
            }

            // Check neighbors
            List<int> neighbors = CheckNeighbors(currentCell);

            if (neighbors.Count == 0) 
            {
                if (path.Count == 0)
                {
                    break;
                }
                else 
                { 
                    currentCell = path.Pop();
                }
            }
            else
            {
                path.Push(currentCell);

                int newCell = neighbors[Random.Range(0, neighbors.Count)];

                if(newCell > currentCell)
                {
                    //down or right
                    if (newCell -1 == currentCell)
                    {
                        board[currentCell].status[2] = true;
                        currentCell = newCell;
                        board[currentCell].status[3] = true;
                    }
                    else
                    {
                        board[currentCell].status[1] = true;
                        currentCell = newCell;
                        board[currentCell].status[0] = true;
                    }
                }
                else
                {
                    //up or left
                    if (newCell + 1 == currentCell)
                    {
                        board[currentCell].status[3] = true;
                        currentCell = newCell;
                        board[currentCell].status[2] = true;
                    }
                    else
                    {
                        board[currentCell].status[0] = true;
                        currentCell = newCell;
                        board[currentCell].status[1] = true;
                    }
                }
            }
        }
        GenerateDungeon();
    }

    List<int> CheckNeighbors(int cell)
    {
        List<int> neighbors = new List<int>();

        //check up
        if (cell - size.x >= 0 && !board[Mathf.FloorToInt(cell - size.x)].visited)
        {
            neighbors.Add(Mathf.FloorToInt(cell - size.x));
        }

        //check down
        if (cell + size.x < board.Count && !board[Mathf.FloorToInt(cell + size.x)].visited)
        {
            neighbors.Add(Mathf.FloorToInt(cell + size.x));
        }

        //check right
        if ((cell+1) % size.x != 0 && !board[Mathf.FloorToInt(cell + 1)].visited)
        {
            neighbors.Add(Mathf.FloorToInt(cell + 1));
        }

        //check left
        if (cell % size.x != 0 && !board[Mathf.FloorToInt(cell - 1)].visited)
        {
            neighbors.Add(Mathf.FloorToInt(cell + 1));
        }

        return neighbors;
    }
}
