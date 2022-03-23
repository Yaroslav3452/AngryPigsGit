using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AiMoving : MonoBehaviour
{
    public int currentIdx;
    public int deltaTargetIdx = -999;
    public int hp = 100;
    public bool isStunned = false;
    public int damage = 1;
    [SerializeField] float speed = .1f;
    [SerializeField] private List<int> path = new List<int>();
    [SerializeField] private bool angry;
    [SerializeField] private Sprite down;
    [SerializeField] private Sprite up;
    [SerializeField] private Sprite left;
    [SerializeField] private Sprite right;
    [SerializeField] private Sprite downAngry;
    [SerializeField] private Sprite upAngry;
    [SerializeField] private Sprite leftAngry;
    [SerializeField] private Sprite rightAngry;
    [SerializeField] private Sprite dirty;
    private SpriteRenderer _spriteRenderer;
    private float _stunCounter;

    public void GeneratePath()
    {
        var target = Random.Range(0, GraphController.Current.adjacent.Count);
        while (GraphController.Current.IsStone(target) && target == currentIdx)
        {
            target = Random.Range(0, GraphController.Current.adjacent.Count);
        }

        path.AddRange(Pathfinding.Current.BuildPath(currentIdx, target));
    }

    private void Stun()
    {
        isStunned = true;
        _spriteRenderer.sprite = dirty;
    }
    private void CheckForThisCell()
    {
        if (BombManager.Current.bombPlaces.Contains(currentIdx))
        {
            Destroy(BombManager.Current.bombs[BombManager.Current.bombPlaces.IndexOf(currentIdx)]);
            BombManager.Current.bombs.RemoveAt(BombManager.Current.bombPlaces.IndexOf(currentIdx));
            BombManager.Current.bombPlaces.Remove(currentIdx);
            hp -= 10;
            Debug.Log("Kaboom");
            Stun();
        }
    }

    private void SmoothMoving()
    {
        if (deltaTargetIdx == -999)
        {
            if (path.Count == 0) return;
            deltaTargetIdx = path[0];
        }

        transform.position = Vector3.MoveTowards(transform.position,
            GraphController.Current.indexToPosition[deltaTargetIdx], speed);
        if (GraphController.Current.adjacent[currentIdx].down == deltaTargetIdx)
        {
            _spriteRenderer.sprite = down;
        }
        else if (GraphController.Current.adjacent[currentIdx].up == deltaTargetIdx)
        {
            _spriteRenderer.sprite = up;
        }
        else if (GraphController.Current.adjacent[currentIdx].left == deltaTargetIdx)
        {
            _spriteRenderer.sprite = left;
        }
        else if (GraphController.Current.adjacent[currentIdx].right == deltaTargetIdx)
        {
            _spriteRenderer.sprite = right;
        }

        if (transform.position == GraphController.Current.indexToPosition[deltaTargetIdx])
        {
            path.Remove(deltaTargetIdx);
            currentIdx = deltaTargetIdx;
            CheckForThisCell();
            deltaTargetIdx = -999;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(GraphController.Current.indexToPosition[currentIdx], .3f);
    }

    private void Update()
    {
        if (isStunned)
        {
            _stunCounter += Time.deltaTime;
            if (_stunCounter >= .4)
            {
                _stunCounter = 0f;
                isStunned = false;
            }
        }
        else
        {
            if (path.Count == 0) GeneratePath();
            if (hp <= 0)
            {
                if (!angry) SetAngry();
                else PlayDeathAnimation();
            }

            SmoothMoving();
        }
    }

    private void PlayDeathAnimation()
    {
        if (AgentManager.Current.dogs.Contains(this)) AgentManager.Current.dogs.Remove(this);
        if (AgentManager.Current.farmer == this) AgentManager.Current.farmer = null;
        Destroy(gameObject);
        AgentManager.Current.CheckForVictory();
    }

    private void SetAngry()
    {
        angry = true;
        hp = 100;
        speed *= 2f;
        up = upAngry;
        down = downAngry;
        right = rightAngry;
        left = leftAngry;
    }

    private void Start()
    {
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }
}