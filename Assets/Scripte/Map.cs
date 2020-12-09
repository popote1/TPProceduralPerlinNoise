using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class Map : MonoBehaviour
{

    [Header("Prefabs")] public GameObject BlocPrefab;
    public GameObject CenterPoint;

    [Header("Parameters")] [Range(0, 1)] public float ThresHold = 0.5f;
    public float Modifier;
    public int MapXSize, MapZSize;
    public int GapX, GapZ;
    public Vector2 Offset;

    [Header("References")] public Camera Camera;

    public Cell[,] Matrix;
    private List<Room> _rooms=new List<Room>();
    private float[,] _perlinMatrix;
    private readonly List<GameObject> _blocs = new List<GameObject>();

    [Header("Step by step Room Analizer")] public Vector2Int OrigineCell;

    private List<Cell> _openList = new List<Cell>();
    private List<Cell> _closeList = new List<Cell>();
    private List<Cell> _newCells = new List<Cell>();

    public void Generate()
    {
        ClearMap();
        SetupCamera();
        BlocsGeneration();
    }

    public void SetupCamera()
    {
        Camera.transform.position =
            new Vector3((float) MapXSize / 2, Camera.transform.position.y, (float) MapZSize / 2);
    }

    public void BlocsGeneration()
    {
        Matrix = new Cell[MapXSize, MapZSize];
        _perlinMatrix = PerlinNoise.Cave(new float[MapXSize, MapZSize], Modifier, true, Offset);
        for (int i = 0; i < MapXSize; i++)
        {
            for (int j = 0; j < MapZSize; j++)
            {
                float value = _perlinMatrix[i, j];
                GameObject instantiate = Instantiate(BlocPrefab, new Vector3(i * GapX, 0, j * GapZ),
                    Quaternion.identity, transform);
                if (value < ThresHold)
                {
                    instantiate.GetComponent<MeshRenderer>().material.color = Color.white;
                }
                else
                {
                    instantiate.GetComponent<MeshRenderer>().material.color = Color.black;
                }

                _blocs.Add(instantiate);
                Matrix[i, j] = new Cell(new Vector2Int(i, j), instantiate);
            }
        }
    }

    public void ClearMap()
    {
        foreach (GameObject bloc in _blocs)
        {
            Destroy(bloc);
        }

        _blocs.Clear();
    }

    public void RoomDetector()
    {
        List<Cell> cellTraiter = new List<Cell>();
        Color color = Color.cyan;
        for (int x = 0; x < MapXSize; x++)
        {
            for (int y = 0; y < MapZSize; y++)
            {
                if (!cellTraiter.Contains(Matrix[x, y]))
                {
                    if (Matrix[x, y].Material.color == Color.white)
                    {
                       // Debug.Log("La case " + x + " ; " + y);
                        

                        Room room = new Room( GetNewRoom(Matrix[x, y]));
                        // RoomAnaliser(Matrix[x,y],out List<Cell> newRoom,out List<Cell>cellsCheck);
                        _rooms.Add(room);
                        foreach (Cell cell in room.Cells)
                        {
                            cellTraiter.Add(cell);
                        }
                    }
                }

            }
        }

       Debug.Log("Il ya " + _rooms.Count + " salles");
        foreach (Room room in _rooms)
        {
            Debug.Log(" salle de "+room.Count+" ou "+room.Cells.Count+ "cases   vas être coloter en "+color);
            foreach (Cell cell in room.Cells)
            {
                cell.Material.color = color;
            }

            if (color == Color.cyan)
            {
                color = Color.red;
            }
            else if (color == Color.red)
            {
                color = Color.green;
            }
            else if (color == Color.green)
            {
                color = Color.yellow;
            }
            else if (color == Color.yellow)
            {
                color = Color.grey;
            }
            else if (color == Color.gray)
            {
                color = Color.magenta;
            }
            else if (color == Color.magenta)
            {
                color = Color.cyan;
            }
            

        }
    
    }

    private void RoomAnaliser(Cell cell, out List<Cell> roomCell, out List<Cell> cellCheck)
    {
        roomCell = new List<Cell>();
        cellCheck = new List<Cell>();
        roomCell.Add(cell);
        Debug.Log("Anilise de la room");

        while (roomCell.Count >= cellCheck.Count)
        {
            Debug.Log(roomCell.Count + " case font partie de la salle   " + cellCheck.Count + "   On était traiter");

            List<Cell> newRoomCells = new List<Cell>();
            foreach (Cell analisCell in roomCell)
            {
                Debug.Log("la casse traiter a comme coordoner" + analisCell.Position);
                if (!cellCheck.Contains(analisCell))
                {
                    if (analisCell.Position.y + 1 < MapXSize)
                    {
                        if (Matrix[analisCell.Position.x, analisCell.Position.y + 1].Material.color == Color.white &&
                            !roomCell.Contains(Matrix[analisCell.Position.x, analisCell.Position.y + 1]) &&
                            !newRoomCells.Contains(Matrix[analisCell.Position.x, analisCell.Position.y + 1]))
                        {
                            newRoomCells.Add(Matrix[analisCell.Position.x, analisCell.Position.y]);
                        }
                    }

                    if (analisCell.Position.y - 1 >= 0)
                    {
                        if (Matrix[analisCell.Position.x, analisCell.Position.y - 1].Material.color == Color.white &&
                            !roomCell.Contains(Matrix[analisCell.Position.x, analisCell.Position.y - 1]) &&
                            !newRoomCells.Contains(Matrix[analisCell.Position.x, analisCell.Position.y - 1]))
                        {
                            newRoomCells.Add(Matrix[analisCell.Position.x, analisCell.Position.y]);
                        }
                    }

                    if (analisCell.Position.x + 1 < MapZSize)
                    {
                        if (Matrix[analisCell.Position.x + 1, analisCell.Position.y].Material.color == Color.white &&
                            !roomCell.Contains(Matrix[analisCell.Position.x + 1, analisCell.Position.y]) &&
                            !newRoomCells.Contains(Matrix[analisCell.Position.x + 1, analisCell.Position.y]))
                        {
                            newRoomCells.Add(Matrix[analisCell.Position.x, analisCell.Position.y]);
                        }
                    }

                    if (analisCell.Position.x - 1 >= 0)
                    {
                        if (Matrix[analisCell.Position.x - 1, analisCell.Position.y - 1].Material.color ==
                            Color.white &&
                            !roomCell.Contains(Matrix[analisCell.Position.x - 1, analisCell.Position.y]) &&
                            !newRoomCells.Contains(Matrix[analisCell.Position.x - 1, analisCell.Position.y]))
                        {
                            newRoomCells.Add(Matrix[analisCell.Position.x, analisCell.Position.y]);
                        }
                    }

                    cellCheck.Add(analisCell);
                }
            }

            foreach (Cell newRoomCell in newRoomCells)
            {
                roomCell.Add(newRoomCell);
            }

            newRoomCells.Clear();
        }
    }

    public void StepByStepRoomAnaliser()
    {
        if (_openList.Count != 0)
        {
            foreach (Cell cell in _openList)
            {
                if (cell.Position.y + 1 < MapZSize)
                {
                    if (Matrix[cell.Position.x, cell.Position.y + 1].Material.color == Color.white
                        && !_openList.Contains(Matrix[cell.Position.x, cell.Position.y + 1])
                        && !_closeList.Contains(Matrix[cell.Position.x, cell.Position.y + 1])
                        && !_newCells.Contains(Matrix[cell.Position.x, cell.Position.y + 1]))
                    {
                        Matrix[cell.Position.x, cell.Position.y + 1].Material.color = Color.green;
                        _newCells.Add(Matrix[cell.Position.x, cell.Position.y + 1]);
                    }
                }

                if (cell.Position.y - 1 >= 0)
                {
                    if (Matrix[cell.Position.x, cell.Position.y - 1].Material.color == Color.white
                        && !_openList.Contains(Matrix[cell.Position.x, cell.Position.y - 1])
                        && !_closeList.Contains(Matrix[cell.Position.x, cell.Position.y - 1])
                        && !_newCells.Contains(Matrix[cell.Position.x, cell.Position.y - 1]))
                    {
                        Matrix[cell.Position.x, cell.Position.y - 1].Material.color = Color.green;
                        _newCells.Add(Matrix[cell.Position.x, cell.Position.y - 1]);
                    }
                }

                if (cell.Position.x + 1 < MapXSize)
                {

                    if (Matrix[cell.Position.x + 1, cell.Position.y].Material.color == Color.white
                        && !_openList.Contains(Matrix[cell.Position.x + 1, cell.Position.y])
                        && !_closeList.Contains(Matrix[cell.Position.x + 1, cell.Position.y])
                        && !_newCells.Contains(Matrix[cell.Position.x + 1, cell.Position.y]))
                    {
                        Matrix[cell.Position.x + 1, cell.Position.y].Material.color = Color.green;
                        _newCells.Add(Matrix[cell.Position.x + 1, cell.Position.y]);
                    }
                }

                if (cell.Position.x - 1 >= 0)
                {

                    if (Matrix[cell.Position.x - 1, cell.Position.y].Material.color == Color.white
                        && !_openList.Contains(Matrix[cell.Position.x - 1, cell.Position.y])
                        && !_closeList.Contains(Matrix[cell.Position.x - 1, cell.Position.y])
                        && !_newCells.Contains(Matrix[cell.Position.x - 1, cell.Position.y]))
                    {
                        Matrix[cell.Position.x - 1, cell.Position.y].Material.color = Color.green;
                        _newCells.Add(Matrix[cell.Position.x - 1, cell.Position.y]);
                    }
                }

                cell.Material.color = Color.blue;

                _closeList.Add(cell);
            }

            _openList.Clear();

            foreach (Cell cell in _newCells)
            {
                _openList.Add(cell);
            }

            _newCells.Clear();
            if (_openList.Count == 0)
            {
                Debug.Log("AnaliseFini");
            }
        }
        else
        {
            if (Matrix[OrigineCell.x, OrigineCell.y].Material.color == Color.white)
            {
                Matrix[OrigineCell.x, OrigineCell.y].Material.color = Color.blue;
                _openList.Add(Matrix[OrigineCell.x, OrigineCell.y]);
            }
        }


    }

   
    public List<Cell> GetNewRoom(Cell origineCell)
    {
        _openList.Clear();
        _closeList.Clear();
        _newCells.Clear();
        do
        {
            if (_openList.Count != 0)
            {
                foreach (Cell cell in _openList)
                {
                    if (cell.Position.y + 1 < MapZSize)
                    {
                        if (Matrix[cell.Position.x, cell.Position.y + 1].Material.color == Color.white
                            && !_openList.Contains(Matrix[cell.Position.x, cell.Position.y + 1])
                            && !_closeList.Contains(Matrix[cell.Position.x, cell.Position.y + 1])
                            && !_newCells.Contains(Matrix[cell.Position.x, cell.Position.y + 1]))
                        {
                            Matrix[cell.Position.x, cell.Position.y + 1].Material.color = Color.green;
                            _newCells.Add(Matrix[cell.Position.x, cell.Position.y + 1]);
                        }
                    }

                    if (cell.Position.y - 1 >= 0)
                    {
                        if (Matrix[cell.Position.x, cell.Position.y - 1].Material.color == Color.white
                            && !_openList.Contains(Matrix[cell.Position.x, cell.Position.y - 1])
                            && !_closeList.Contains(Matrix[cell.Position.x, cell.Position.y - 1])
                            && !_newCells.Contains(Matrix[cell.Position.x, cell.Position.y - 1]))
                        {
                            Matrix[cell.Position.x, cell.Position.y - 1].Material.color = Color.green;
                            _newCells.Add(Matrix[cell.Position.x, cell.Position.y - 1]);
                        }
                    }

                    if (cell.Position.x + 1 < MapXSize)
                    {

                        if (Matrix[cell.Position.x + 1, cell.Position.y].Material.color == Color.white
                            && !_openList.Contains(Matrix[cell.Position.x + 1, cell.Position.y])
                            && !_closeList.Contains(Matrix[cell.Position.x + 1, cell.Position.y])
                            && !_newCells.Contains(Matrix[cell.Position.x + 1, cell.Position.y]))
                        {
                            Matrix[cell.Position.x + 1, cell.Position.y].Material.color = Color.green;
                            _newCells.Add(Matrix[cell.Position.x + 1, cell.Position.y]);
                        }
                    }

                    if (cell.Position.x - 1 >= 0)
                    {

                        if (Matrix[cell.Position.x - 1, cell.Position.y].Material.color == Color.white
                            && !_openList.Contains(Matrix[cell.Position.x - 1, cell.Position.y])
                            && !_closeList.Contains(Matrix[cell.Position.x - 1, cell.Position.y])
                            && !_newCells.Contains(Matrix[cell.Position.x - 1, cell.Position.y]))
                        {
                            Matrix[cell.Position.x - 1, cell.Position.y].Material.color = Color.green;
                            _newCells.Add(Matrix[cell.Position.x - 1, cell.Position.y]);
                        }
                    }

                    cell.Material.color = Color.blue;

                    _closeList.Add(cell);
                }

                _openList.Clear();

                foreach (Cell cell in _newCells)
                {
                    _openList.Add(cell);
                }

                _newCells.Clear();
            }
            else
            {
                _openList.Add(origineCell);
            }


        } while (_openList.Count != 0);
        Debug.Log("AnaliseFini avec un salle de "+_closeList.Count+ " cases");
        return _closeList;
        
    }

    public void Createjoinction()
    {
        
    }


    public class Cell
    {
        public Vector2Int Position;
        public GameObject GameObject;
        public Material Material;

        public Cell(Vector2Int position, GameObject gameObject)
        {
            Position = position;
            GameObject = gameObject;
            Material = gameObject.GetComponent<MeshRenderer>().material;
        }
    }

    public class Room
    {
        public List<Cell> Cells = new List<Cell>();
        public int Count;
        public Vector2 Center;

        public Room(List<Cell> cells)
        {
            Vector2 sum = Vector2.zero;
            foreach (Cell cell in cells)
            {
                Cells.Add(cell);
                sum += cell.Position;
            }
            Count = Cells.Count;
            Center = sum / Count;
            GameObject centerObject=Instantiate(cells[0].GameObject, new Vector3(Center.x, 0.1f,Center.y), Quaternion.identity);
            centerObject.GetComponent<MeshRenderer>().material.color =Color.blue;

        }
    }
}