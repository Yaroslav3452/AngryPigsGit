
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Pathfinding : MonoBehaviour
{
    public static Pathfinding Current;
    public GraphController graphController;
    public List<int> path = new List<int>();

    public List<int> BuildPath(int startIdx, int endIdx)
    {
        path = new List<int>();
        Queue<int> nodeQueue = new Queue<int>();
        int nodeCount = graphController.indexToPosition.Count;
        bool[] isVisited = new bool[nodeCount];
        int[] distances = new int[nodeCount];
        for (int i = 0; i < nodeCount; i++)
        {
            distances[i] = -1;
        }
        nodeQueue.Enqueue(startIdx);
        isVisited[startIdx] = true;
        distances[startIdx] = 0;
        while (nodeQueue.Count != 0)
        {
            int idx = nodeQueue.Dequeue();
            for (int i = 0; i < 4; i++)
            {
                var neighbour = GraphController.Current.adjacent[idx][i];
                if (neighbour == -1) continue;
                if (!isVisited[neighbour])
                {
                    isVisited[neighbour] = true;
                    distances[neighbour] = GetDistance(distances, idx, neighbour); // стоимость движения по клеткам 
                    nodeQueue.Enqueue(neighbour);
                }
                else
                {
                    if (distances[neighbour] > GetDistance(distances, idx, neighbour))
                    {
                        Debug.Log("Something wrong here! Call for Bobr!");
                        distances[neighbour] = GetDistance(distances, idx, neighbour);
                    }
                }
            }
        }
        if (distances[endIdx] == -1) return path;
        var index = endIdx;
        path.Add(index);
        while (index != startIdx)
        {
            for (int i = 0; i < 4; i++)
            {
                var neighbour = GraphController.Current.adjacent[index][i];
                if(neighbour == -1) continue;
                if (distances[index] - distances[neighbour] == 1)
                {
                    path.Add(neighbour);
                    index = neighbour;
                }
            }
        }
        path.Reverse();
        path.RemoveAt(0);
        return path;
    }
    
    private int GetDistance(int[] distances, int from, int to)
    {
        return distances[from] + 1;
    }

    private void Start()
    {
        graphController = gameObject.GetComponent<GraphController>();
    }

    private void Awake()
    {
        Current = this;
    }
}
