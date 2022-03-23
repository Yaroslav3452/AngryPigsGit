using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BombManager : MonoBehaviour
{
        public static BombManager Current;
        public int bombsInReserve = 10;
        public List<int> bombPlaces = new List<int>();
        public List<GameObject> bombs = new List<GameObject>();
        [SerializeField] private TextMeshProUGUI bombsCountText;
        [SerializeField] private GameObject bombPrefab;
        [SerializeField] private float bombsAddTimer;

        public void ClearBombs()
        {
                foreach (var bomb in bombs)
                {
                        Destroy(bomb);
                }
                bombs.Clear();
                bombPlaces.Clear();
        }
        public void SpawnBomb()
        {
                if(bombsInReserve <= 0) return;
                var bombPosIdx = PigMovement.Current.currentPositionIdx;
                if (bombPlaces.Contains(bombPosIdx)) return;
                var bomb = Instantiate(bombPrefab, GraphController.Current.indexToPosition[bombPosIdx], Quaternion.identity,
                        transform);
                bombs.Add(bomb);
                bombsInReserve--;
                bombPlaces.Add(bombPosIdx);
                UpdateBombsCountText();
        }

        public void BlowUpBomb()
        {

        }

        private void FixedUpdate()
        {
                bombsAddTimer += Time.fixedDeltaTime;
                if (bombsAddTimer >= 1.5f)
                {
                        bombsInReserve++;
                        bombsAddTimer = 0f;
                        UpdateBombsCountText();
                }
        }

        public void UpdateBombsCountText()
        {
                bombsCountText.text = "bombs: " + bombsInReserve;
        }
        private void Awake()
        {
                Current = this;
                UpdateBombsCountText();
        }
}