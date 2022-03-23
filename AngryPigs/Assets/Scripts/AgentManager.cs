using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AgentManager : MonoBehaviour
{
    public static AgentManager Current;
    public List<AiMoving> dogs = new List<AiMoving>();
    public AiMoving farmer;
    public PigMovement pig;
    [SerializeField] private GameObject dogPrefab;
    [SerializeField] private GameObject pigPrefab;
    [SerializeField] private GameObject farmerPrefab;

    public void CheckForVictory()
    {
        if (dogs.Count == 0 && farmer == null)
        {
            CanvasesManager.Current.OpenLooseMenu();
        }
    }
    public bool IsHereDog(int idx)
    {
        foreach (var dog in dogs)
        {
            if (dog.currentIdx == idx) return true;
        }

        return false;
    }

    public bool IsHereFarmer(int idx)
    {
        if (farmer != null && farmer.currentIdx == idx) return true;
        return false;
    }

    public void CreateAgents()
    {
        dogs.Clear();
        var scripts = FindObjectsOfType<AiMoving>();
        foreach (var script in scripts)
        {
            Destroy(script.gameObject);
        }
        if (pig != null) Destroy(pig.gameObject);
        CreateDog(3);
        CreateFarmer();
        CreatePig();
    }

    private void CreatePig()
    {
        var targetPointIdx = ReturnValidPositionForRespawn();
        var go = Instantiate(pigPrefab, GraphController.Current.indexToPosition[targetPointIdx], Quaternion.identity);
        var script = go.GetComponent<PigMovement>();
        script.currentPositionIdx = targetPointIdx;
        pig = script;
    }

    private void CreateDog(int count)
    {
        for (int i = 0; i < count; i++)
        {
            var targetPointIdx = ReturnValidPositionForRespawn();
            var go = Instantiate(dogPrefab, GraphController.Current.indexToPosition[targetPointIdx],
                Quaternion.identity);
            var script = go.GetComponent<AiMoving>();
            script.currentIdx = targetPointIdx;
            dogs.Add(script);
            script.GeneratePath();
        }
    }

    private bool IsValidPosition(int posIdx)
    {
        if (GraphController.Current.IsStone(posIdx)) return false;
        foreach (var bush in GraphController.Current.generatedBush)
            if (bush == posIdx)
                return false;
        
        foreach (var dog in dogs)
            if (dog.currentIdx == posIdx)
                return false;
        if (farmer != null && farmer.currentIdx == posIdx) return false;
        if (pig != null && pig.currentPositionIdx == posIdx) return false;
        return true;
    }

    private int ReturnValidPositionForRespawn()
    {
        var idx = Random.Range(0, GraphController.Current.adjacent.Count);
        while (!IsValidPosition(idx))
        {
            idx = Random.Range(0, GraphController.Current.adjacent.Count);
        }

        return idx;
    }

    private void CreateFarmer()
    {
        var targetPointIdx = ReturnValidPositionForRespawn();
        var go = Instantiate(farmerPrefab, GraphController.Current.indexToPosition[targetPointIdx],
            Quaternion.identity);
        var script = go.GetComponent<AiMoving>();
        script.currentIdx = targetPointIdx;
        farmer = script;
        script.GeneratePath();
    }


    private void Awake()
    {
        Current = this;
    }
}