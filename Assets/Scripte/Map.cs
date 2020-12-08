using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class Map : MonoBehaviour {

    [Header("Prefabs")] 
    public GameObject BlocPrefab;

    [Header("Parameters")] 
    [Range(0,1)]public float ThresHold=0.5f;
    public float Modifier;
    public int MapXSize, MapZSize;
    public int GapX, GapZ;
    public Vector2 Offset;

    [Header("References")] 
    public Camera Camera;

    public Cell[,] Matrix;
    private List<List<Cell>> _rooms;
    private float[,] _perlinMatrix;
    private readonly List<GameObject> _blocs = new List<GameObject>();


    public void Generate() {
        ClearMap();
        SetupCamera();
        BlocsGeneration();
    }

    /// <summary>
    /// Code teste
    /// </summary>

    void StartAnalyse()
    {
        StartCoroutine(Analyse());
    }

    IEnumerator<List<Cell>> Analyse(Cell cell )
    {
        List<Cell>roomCell = new List<Cell>();
        List<Cell>cellCheck=new List<Cell>();
        roomCell.Add(cell);
        Debug.Log("Anilise de la room");
        
        while (roomCell.Count>=cellCheck.Count)
        {
            Debug.Log(roomCell.Count+" case font partie de la salle   "+cellCheck.Count+"   On était traiter");
            
            List<Cell> newRoomCells= new List<Cell>();
            foreach (Cell analisCell in roomCell)
            {
                Debug.Log("la casse traiter a comme coordoner" + analisCell.Position);
                if (!cellCheck.Contains(analisCell))
                {
                    if (analisCell.Position.y+1 <MapXSize)
                    {
                        if (Matrix[analisCell.Position.x, analisCell.Position.y + 1].Material.color == Color.white && !roomCell.Contains(Matrix[analisCell.Position.x, analisCell.Position.y + 1])&& !newRoomCells.Contains(Matrix[analisCell.Position.x, analisCell.Position.y + 1]))
                        {
                            newRoomCells.Add(Matrix[analisCell.Position.x, analisCell.Position.y]);
                        }
                    }
                    
                    if (analisCell.Position.y - 1 >= 0)
                    {
                        if (Matrix[analisCell.Position.x, analisCell.Position.y - 1].Material.color == Color.white && !roomCell.Contains(Matrix[analisCell.Position.x, analisCell.Position.y - 1])&& !newRoomCells.Contains(Matrix[analisCell.Position.x, analisCell.Position.y - 1]))
                        {
                            newRoomCells.Add(Matrix[analisCell.Position.x, analisCell.Position.y]);
                        }
                    }

                    if (analisCell.Position.x + 1 < MapZSize)
                    {
                        if (Matrix[analisCell.Position.x + 1, analisCell.Position.y].Material.color == Color.white && !roomCell.Contains(Matrix[analisCell.Position.x + 1, analisCell.Position.y])&& !newRoomCells.Contains(Matrix[analisCell.Position.x+1, analisCell.Position.y ]))
                        {
                            newRoomCells.Add(Matrix[analisCell.Position.x, analisCell.Position.y]);
                        }
                    }

                    if (analisCell.Position.x - 1 >= 0)
                    {
                        if (Matrix[analisCell.Position.x - 1, analisCell.Position.y - 1].Material.color == Color.white && !roomCell.Contains(Matrix[analisCell.Position.x - 1, analisCell.Position.y])&& !newRoomCells.Contains(Matrix[analisCell.Position.x-1, analisCell.Position.y ]))
                        {
                            newRoomCells.Add(Matrix[analisCell.Position.x, analisCell.Position.y]);
                        }
                    }

                    cellCheck.Add(analisCell);
                    yield return new WaitForSeconds(0.1f);
                }
            }

            foreach (Cell newRoomCell in newRoomCells)
            {
                roomCell.Add(newRoomCell);
            }
            newRoomCells.Clear();
x        }
    }

    public void SetupCamera() {
        Camera.transform.position = new Vector3((float) MapXSize / 2, Camera.transform.position.y, (float) MapZSize / 2);
    }
    
    public void BlocsGeneration()
    {
        Matrix = new Cell[MapXSize,MapZSize];
        _perlinMatrix = PerlinNoise.Cave(new float[MapXSize,MapZSize], Modifier, true,Offset);
        for (int i = 0; i < MapXSize; i++) {
            for (int j = 0; j < MapZSize; j++)
            {
                float value = _perlinMatrix[i, j];
                GameObject instantiate = Instantiate(BlocPrefab, new Vector3(i * GapX, 0, j * GapZ), Quaternion.identity, transform);
                if (value <ThresHold)
                {
                    instantiate.GetComponent<MeshRenderer>().material.color = Color.white;
                }
                else
                {
                    instantiate.GetComponent<MeshRenderer>().material.color = Color.black;
                }

                _blocs.Add(instantiate);
                Matrix[i,j]=new Cell(new Vector2Int(i,j),instantiate);
            }
        }
    }

    public void ClearMap() {
        foreach (GameObject bloc in _blocs) {
            Destroy(bloc);
        }
        _blocs.Clear();
    }

    public void RoomDetector()
    {
        List<Cell> cellTraiter = new List<Cell>();
        Color color = Color.blue;
        for (int x = 0; x < MapXSize; x++) {
            for (int y = 0; y < MapZSize; y++) {
                if (!cellTraiter.Contains(Matrix[x, y])) {
                    if (Matrix[x,y].Material.color==Color.white) {
                        Debug.Log("La case "+x+" ; "+y);
                        RoomAnaliser(Matrix[x,y],out List<Cell> newRoom,out List<Cell>cellsCheck);
                        _rooms.Add(newRoom);
                        foreach (Cell cell in cellsCheck) { 
                            cellTraiter.Add(cell);
                        }
                    }
                }
                
            }
        }

        foreach (List<Cell> room in _rooms) {
            foreach (Cell cell in room)
            {
                cell.Material.color = color;
            }

            if (color == Color.blue)
            {
                color=Color.red;
            }
            else if (color == Color.red)
            {
                color = Color.green;
            }else if(color ==Color.green)
            {
                color = Color.blue;
            }
            
        }
    }

    private void RoomAnaliser(Cell cell , out List<Cell> roomCell,out List<Cell>cellCheck)
    {
        roomCell = new List<Cell>();
        cellCheck=new List<Cell>();
        roomCell.Add(cell);
        Debug.Log("Anilise de la room");
        
        while (roomCell.Count>=cellCheck.Count)
        {
            Debug.Log(roomCell.Count+" case font partie de la salle   "+cellCheck.Count+"   On était traiter");
            
            List<Cell> newRoomCells= new List<Cell>();
            foreach (Cell analisCell in roomCell)
            {
                Debug.Log("la casse traiter a comme coordoner" + analisCell.Position);
                if (!cellCheck.Contains(analisCell))
                {
                    if (analisCell.Position.y+1 <MapXSize)
                    {
                        if (Matrix[analisCell.Position.x, analisCell.Position.y + 1].Material.color == Color.white && !roomCell.Contains(Matrix[analisCell.Position.x, analisCell.Position.y + 1])&& !newRoomCells.Contains(Matrix[analisCell.Position.x, analisCell.Position.y + 1]))
                        {
                            newRoomCells.Add(Matrix[analisCell.Position.x, analisCell.Position.y]);
                        }
                    }

                    if (analisCell.Position.y - 1 >= 0)
                    {
                        if (Matrix[analisCell.Position.x, analisCell.Position.y - 1].Material.color == Color.white && !roomCell.Contains(Matrix[analisCell.Position.x, analisCell.Position.y - 1])&& !newRoomCells.Contains(Matrix[analisCell.Position.x, analisCell.Position.y - 1]))
                        {
                            newRoomCells.Add(Matrix[analisCell.Position.x, analisCell.Position.y]);
                        }
                    }

                    if (analisCell.Position.x + 1 < MapZSize)
                    {
                        if (Matrix[analisCell.Position.x + 1, analisCell.Position.y].Material.color == Color.white && !roomCell.Contains(Matrix[analisCell.Position.x + 1, analisCell.Position.y])&& !newRoomCells.Contains(Matrix[analisCell.Position.x+1, analisCell.Position.y ]))
                        {
                            newRoomCells.Add(Matrix[analisCell.Position.x, analisCell.Position.y]);
                        }
                    }

                    if (analisCell.Position.x - 1 >= 0)
                    {
                        if (Matrix[analisCell.Position.x - 1, analisCell.Position.y - 1].Material.color == Color.white && !roomCell.Contains(Matrix[analisCell.Position.x - 1, analisCell.Position.y])&& !newRoomCells.Contains(Matrix[analisCell.Position.x-1, analisCell.Position.y ]))
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