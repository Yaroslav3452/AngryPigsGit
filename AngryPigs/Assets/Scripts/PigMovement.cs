using System.Collections.Generic;
using SimpleInputNamespace;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class PigMovement : MonoBehaviour
{
        public static PigMovement Current;
        public int currentPositionIdx;
        public int currentTargetIdx = -999;
        public bool debugDrawWindow = false;
        [SerializeField] private int hpCount = 10;
        [SerializeField] private List<Sprite> pigSprites = new List<Sprite>();
        [SerializeField] private Vector3 direction;
        private Slider _hpSlider;
        private SpriteRenderer _spriteRenderer;
        private Dpad _dpad;
        private bool _isInvulnerable = false;
        private float _invulnerableTimer = 0f;
        private float _maxHp;

        public void ContactWithEnemy(int damage)
        {
                if (_isInvulnerable) return;
                hpCount -= damage;
                if (_hpSlider != null) _hpSlider.value = hpCount;
                if (hpCount <= 0)
                {
                        CanvasesManager.Current.OpenLooseMenu();
                }

                _spriteRenderer.color = Color.red;
                _isInvulnerable = true;
        }

        private void OnDrawGizmos()
        { 
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(GraphController.Current.indexToPosition[currentPositionIdx], .3f);
        }

        private void MovePig(Vector3 directionOfMoving)
        {
                var nextPointIdx = -999;
                if (directionOfMoving == Vector3.up)
                        nextPointIdx = GraphController.Current.adjacent[currentPositionIdx].up;
                else if (directionOfMoving == Vector3.down)
                        nextPointIdx = GraphController.Current.adjacent[currentPositionIdx].down;
                else if (directionOfMoving == Vector3.left)
                        nextPointIdx = GraphController.Current.adjacent[currentPositionIdx].left;
                else if (directionOfMoving == Vector3.right)
                        nextPointIdx = GraphController.Current.adjacent[currentPositionIdx].right;
                if (nextPointIdx == -1) return;
                currentTargetIdx = nextPointIdx;
        }

        private void SmoothMoving()
        {
                transform.position = Vector3.MoveTowards(transform.position,
                        GraphController.Current.indexToPosition[currentTargetIdx], .12f);
                if (transform.position == GraphController.Current.indexToPosition[currentTargetIdx])
                {
                        currentPositionIdx = currentTargetIdx;
                        currentTargetIdx = -999;
                }
        }

        private void CheckForNewPos()
        {
                if (GraphController.Current.generatedBush.Contains(currentPositionIdx) && hpCount < _maxHp)
                {       
                        var idxOfBush = GraphController.Current.generatedBush.IndexOf(currentPositionIdx);
                        Destroy(GraphController.Current.generatedBushGameObjects[idxOfBush]);
                        GraphController.Current.generatedBush.RemoveAt(idxOfBush);
                        GraphController.Current.generatedBushGameObjects.RemoveAt(idxOfBush);
                        BombManager.Current.bombsInReserve += 10;
                        BombManager.Current.UpdateBombsCountText();
                        hpCount += 1;
                        _hpSlider.value = hpCount;
                }
                if (AgentManager.Current.IsHereDog(currentPositionIdx)) ContactWithEnemy(1);
                if (AgentManager.Current.IsHereFarmer(currentPositionIdx)) ContactWithEnemy(3);
        }

        private void Update()
        {
                if (currentTargetIdx != -999) SmoothMoving();
                CheckForNewPos();
        }

        private void FixedUpdate()
        {
                if (_isInvulnerable)
                {
                        _invulnerableTimer += Time.fixedDeltaTime;
                        if (_invulnerableTimer >= .5f)
                        {
                                _invulnerableTimer = 0f;
                                _isInvulnerable = false;
                                _spriteRenderer.color = Color.white;
                        }
                }

                if (currentTargetIdx == -999)
                {
#if UNITY_EDITOR
                        
                        if (Input.GetKey(KeyCode.DownArrow))
                        {
                                MovePig(Vector3.down);
                                _spriteRenderer.sprite = pigSprites[0];
                        }
                        else if (Input.GetKey(KeyCode.UpArrow))
                        {
                                MovePig(Vector3.up);
                                _spriteRenderer.sprite = pigSprites[1];
                        }
                        else if (Input.GetKey(KeyCode.LeftArrow))
                        {
                                MovePig(Vector3.left);
                                _spriteRenderer.sprite = pigSprites[2];
                        }
                        else if (Input.GetKey(KeyCode.RightArrow))
                        {
                                MovePig(Vector3.right);
                                _spriteRenderer.sprite = pigSprites[3];
                        }
#endif
                        if (Mathf.Abs(_dpad.Value.x) > Mathf.Abs(_dpad.Value.y))
                        {
                                if (_dpad.Value.x > 0)
                                {
                                        MovePig(Vector3.right);
                                        _spriteRenderer.sprite = pigSprites[3];
                                }
                                else if (_dpad.Value.x < 0)
                                {
                                        MovePig(Vector3.left);
                                        _spriteRenderer.sprite = pigSprites[2];
                                }
                        }
                        else
                        {
                                if (_dpad.Value.y > 0)
                                {
                                        MovePig(Vector3.up);
                                        _spriteRenderer.sprite = pigSprites[1];
                                }
                                else if (_dpad.Value.y < 0)
                                {
                                        MovePig(Vector3.down);
                                        _spriteRenderer.sprite = pigSprites[0];
                                }
                        }
                }
#if UNITY_EDITOR
                if (Input.GetKey(KeyCode.Space))
                {
                        BombManager.Current.SpawnBomb();
                }
#endif
        }

        private void Start()
        {
                _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
                _hpSlider = FindObjectOfType<Slider>();
                _dpad = FindObjectOfType<Dpad>();
                if (_hpSlider != null)
                {
                        _hpSlider.value = hpCount;
                        _hpSlider.maxValue = hpCount;
                }

                _maxHp = hpCount;
        }

        private void Awake()
        {
                Current = this;
        }
}