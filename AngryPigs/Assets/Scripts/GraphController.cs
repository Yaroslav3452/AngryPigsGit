using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GraphController : MonoBehaviour
{
  public static GraphController Current;
  public bool isNeedToRebuildGraph;
  public bool debugDraw = false;
  public List<Vector3> indexToPosition = new List<Vector3>();
  public Dictionary<Vector3, int> PositionToIndex = new Dictionary<Vector3, int>();
  public List<NodeAdjacent> adjacent = new List<NodeAdjacent>();
  public List<int> generatedBush = new List<int>();
  public List<GameObject> generatedBushGameObjects = new List<GameObject>();
  public List<int> stonePlaces = new List<int>();
  public List<int> bushPlaces = new List<int>();
  [SerializeField] private GameObject stonePrefab;
  [SerializeField] private GameObject bushPrefab;
  [SerializeField] private GameObject bushesStorage;
  [SerializeField] private Transform stonesStorage;

  [Serializable]
  public struct NodeAdjacent
  {
    public int this[int key]
    {
      get
      {
        switch (key)
        {
          case 0: return up;
          case 1: return down;
          case 2: return right;
          case 3: return left;
          default: return -1;
        }
      }
      set
      {
        switch (key)
        {
          case 0:
            up = value;
            break;
          case 1:
            down = value;
            break;
          case 2:
            right = value;
            break;
          case 3:
            left = value;
            break;
        }
      }
    }

    public int up;
    public int down;
    public int right;
    public int left;
  }

  public void GenerateNewObjects()
  {
    GetPointsInChildren();
    GenerateStones();
    CollectBushPlaces();
    GenerateBushes();
  }
  public bool IsStone(int idx)
  {
    for (int i = 0; i < 4; i++)
    {
      if (adjacent[idx][i] != -1) return false;
    }

    return true;
  }
  private void GetPointsInChildren()
  {
    indexToPosition.Clear();
    PositionToIndex.Clear();
    adjacent.Clear();
    var linesCount = transform.childCount;
    var columnsCount = transform.GetChild(0).childCount;
    int lineIdx = 0;
    foreach (Transform line in transform)
    {
      int columnIdx = 0;
      foreach (Transform node in line)
      {
        var position = node.position;
        var nodeIdx = PositionToIndex.Count;
        PositionToIndex.Add(position, nodeIdx);
        indexToPosition.Add(position);
        var nodeAdjacent = new NodeAdjacent();
        if (columnIdx != 0) nodeAdjacent.left = nodeIdx - 1;
        else nodeAdjacent.left = -1;
        if (columnIdx < columnsCount - 1) nodeAdjacent.right = nodeIdx + 1;
        else nodeAdjacent.right = -1;
        if (lineIdx != 0) nodeAdjacent.up = nodeIdx - columnsCount;
        else nodeAdjacent.up = -1;
        if (lineIdx < linesCount - 1) nodeAdjacent.down = nodeIdx + columnsCount;
        else nodeAdjacent.down = -1;
        adjacent.Add(nodeAdjacent);
        columnIdx++;
      }

      lineIdx++;
    }
  }

  private void OnDrawGizmos()
  {
    if (!debugDraw) return;
    Gizmos.color = Color.green;
    for (int i = 0; i < adjacent.Count; i++)
    {
      if (adjacent[i].up >= indexToPosition.Count ||
          adjacent[i].down >= indexToPosition.Count ||
          adjacent[i].right >= indexToPosition.Count ||
          adjacent[i].left >= indexToPosition.Count ||
          i >= indexToPosition.Count)
      {
        Debug.Log("smth wrong");
      }

      if (adjacent[i].up >= 0) Gizmos.DrawLine(indexToPosition[i], indexToPosition[adjacent[i].up]);
      if (adjacent[i].down >= 0) Gizmos.DrawLine(indexToPosition[i], indexToPosition[adjacent[i].down]);
      if (adjacent[i].left >= 0) Gizmos.DrawLine(indexToPosition[i], indexToPosition[adjacent[i].left]);
      if (adjacent[i].right >= 0) Gizmos.DrawLine(indexToPosition[i], indexToPosition[adjacent[i].right]);
    }
  }

  private void GenerateStones()
  {
    for (int i = 0; i < stonesStorage.childCount; i++)
    {
      Destroy(stonesStorage.GetChild(i).gameObject);
    }
    var count = Random.Range(stonePlaces.Count / 2, stonePlaces.Count);
    List<int> places = new List<int>(stonePlaces);
    for (int i = 0; i < count; i++)
    {
      var placeIdx = places[Random.Range(0, places.Count)];
      Instantiate(stonePrefab, indexToPosition[placeIdx], Quaternion.identity, stonesStorage.transform);
      places.Remove(placeIdx);
      var up = adjacent[adjacent[placeIdx].up];
      up.down = -1;
      adjacent[adjacent[placeIdx].up] = up;
      var down = adjacent[adjacent[placeIdx].down];
      down.up = -1;
      adjacent[adjacent[placeIdx].down] = down;
      var left = adjacent[adjacent[placeIdx].left];
      left.right = -1;
      adjacent[adjacent[placeIdx].left] = left;
      var right = adjacent[adjacent[placeIdx].right];
      right.left = -1;
      adjacent[adjacent[placeIdx].right] = right;
      adjacent[placeIdx] = new NodeAdjacent {up = -1, down = -1, left = -1, right = -1};
    }
  }
  private void GenerateBushes()
  {
    generatedBush.Clear();
    foreach (var go in generatedBushGameObjects)
    {
      Destroy(go);
    }

    generatedBushGameObjects.Clear();
    var count = Random.Range(5, 10);
    List<int> places = new List<int>(bushPlaces);
    for (int i = 0; i < count; i++)
    {
      var placeIdx = places[Random.Range(0, places.Count)];
      var go = Instantiate(bushPrefab, indexToPosition[placeIdx], Quaternion.identity, bushesStorage.transform);
      generatedBushGameObjects.Add(go);
      places.Remove(placeIdx);
      generatedBush.Add(placeIdx);
    }
  }

  private void CollectStonePlaces()
  {
    stonePlaces.Clear();
    List<GameObject> stoneSpots = new List<GameObject>(GameObject.FindGameObjectsWithTag("StonePlace"));
    foreach (var stone in stoneSpots)
    {
      if (PositionToIndex.TryGetValue(stone.transform.position, out var posInt)) stonePlaces.Add(posInt);
    }
  }
  private void CollectBushPlaces()
  {
    bushPlaces.Clear();
    for (int i = 0; i < adjacent.Count; i++)
    {
      if(IsStone(i)) continue;
      bushPlaces.Add(i);
    }
  }
  private void Start()
  {
    if (isNeedToRebuildGraph)
    {
      GetPointsInChildren();
      CollectStonePlaces();
    }
  }

  private void Awake()
  {
    Current = this;
  }
}
