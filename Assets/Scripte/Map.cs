using System.Collections.Generic;
using UnityEngine;
using Utils;

public class Map : MonoBehaviour {

    [Header("Prefabs")] 
    public GameObject BlocPrefab;

    [Header("Parameters")] 
    public float Modifier;
    public int MapXSize, MapZSize;
    public int GapX, GapZ;

    [Header("References")] 
    public Camera Camera;
    
    private float[,] _matrix;
    private readonly List<GameObject> _blocs = new List<GameObject>();


    public void Generate() {
        ClearMap();
        SetupCamera();
        BlocsGeneration();
    }

    public void SetupCamera() {
        Camera.transform.position = new Vector3((float) MapXSize / 2, Camera.transform.position.y, (float) MapZSize / 2);
    }
    
    public void BlocsGeneration()
    {
        _matrix = new float[MapXSize,MapZSize];
        _matrix = PerlinNoise.Cave(_matrix, Modifier, true);
        for (int i = 0; i < MapXSize; i++) {
            for (int j = 0; j < MapZSize; j++)
            {
                float value = _matrix[i, j];
                GameObject instantiate = Instantiate(BlocPrefab, new Vector3(i * GapX, 0, j * GapZ), Quaternion.identity, transform);
                instantiate.GetComponent<MeshRenderer>().material.color = Color.white * value;
                _blocs.Add(instantiate);
            }
        }
    }

    public void ClearMap() {
        foreach (GameObject bloc in _blocs) {
            Destroy(bloc);
        }
        _blocs.Clear();
    }

}