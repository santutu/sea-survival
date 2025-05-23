using UnityEngine;
using sea_survival.Scripts.Contracts;
using sea_survival.Scripts.Players;
using System.Collections;
using Sirenix.OdinInspector;

namespace sea_survival.Scripts.Enemies
{
    public class NeutralBird : MonoBehaviour
    {
        [Header("이동 설정")]
        [SerializeField] private float flySpeed = 3f;
        [SerializeField] private float diveSpeed = 15f;
        [SerializeField] private float flyRangeMinX = -8f; // 플레이어 기준 왼쪽 비행 범위 (음수)
        [SerializeField] private float flyRangeMaxX = 8f; // 플레이어 기준 오른쪽 비행 범위 (양수)
        [SerializeField] private float skyHeight = 8f;
        [SerializeField] private float diveDepth = -5f;
        
        [Header("다이브 설정")]
        [SerializeField] private float diveInterval = 5f; // x초마다 다이브
        [SerializeField] private float diveWarningTime = 1f; // 다이브 전 경고 시간
        [SerializeField] private float diveHoldTime = 0.5f; // 최하단에서 머무는 시간
        
        [Header("충돌 데미지")]
        [SerializeField] private float damageToPlayer = 15f;
        [SerializeField] private float damageToEnemies = 30f;
        
        [Header("이펙트")]
        [SerializeField] private GameObject warningEffect; // 다이브 경고 이펙트
        [SerializeField] private GameObject diveEffect; // 다이브 이펙트
        
        private Rigidbody2D _rb;
        private SpriteRenderer _spriteRenderer;
        private Vector3 _startPosition;
        private Vector3 _targetPosition;
        private bool _isDiving = false;
        private bool _isReturning = false;
        private float _diveTimer = 0f;
        private int _flyDirection = 1; // 1: 오른쪽, -1: 왼쪽
        private Player _player;
        
        [SerializeField, ReadOnly] private BirdState _currentState = BirdState.Flying;
        
        private enum BirdState
        {
            Flying,
            Warning,
            Diving,
            Holding,
            Returning
        }
        
        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _player = Player.Ins;
        }
        
        private void Start()
        {
            // Rigidbody2D 설정 (물리 간섭 방지)
            if (_rb != null)
            {
                _rb.gravityScale = 0f; // 중력 비활성화
                _rb.linearDamping = 0f; // 선형 저항 비활성화
                _rb.angularDamping = 0f; // 각 저항 비활성화
                _rb.freezeRotation = true; // 회전 고정
            }
            
            // 시작 위치를 플레이어 기준으로 설정
            if (_player != null)
            {
                float startX = _player.transform.position.x + Random.Range(flyRangeMinX, flyRangeMaxX);
                transform.position = new Vector3(startX, skyHeight, transform.position.z);
            }
            else
            {
                transform.position = new Vector3(0f, skyHeight, transform.position.z);
            }
            
            _startPosition = transform.position;
            _diveTimer = diveInterval;
            
            // 랜덤한 방향으로 시작
            _flyDirection = Random.value > 0.5f ? 1 : -1;
            
            Debug.Log($"중립새 초기화 완료 - 위치: {transform.position}, 방향: {_flyDirection}");
        }
        
        private void Update()
        {
            switch (_currentState)
            {
                case BirdState.Flying:
                    HandleFlying();
                    break;
                case BirdState.Warning:
                    // 경고 상태에서는 움직이지 않음
                    break;
                case BirdState.Diving:
                    HandleDiving();
                    break;
                case BirdState.Holding:
                    // 최하단에서 잠시 머무름
                    break;
                case BirdState.Returning:
                    HandleReturning();
                    break;
            }
            
            // 스프라이트 방향 업데이트
            if (_flyDirection != 0)
            {
                _spriteRenderer.flipX = _flyDirection < 0;
            }
        }
        
        private void HandleFlying()
        {
            if (_player == null)
            {
                _player = Player.Ins;
                if (_player == null) return;
            }
            
            // 플레이어 위치 기준으로 비행 범위 계산
            float playerX = _player.transform.position.x;
            float minX = playerX + flyRangeMinX; // flyRangeMinX는 음수이므로 덧셈
            float maxX = playerX + flyRangeMaxX; // flyRangeMaxX는 양수이므로 덧셈
            
            // 현재 새의 위치가 범위를 크게 벗어났는지 체크
            float currentX = transform.position.x;
            
            // 범위를 벗어났을 때 강제로 범위 안으로 이동
            if (currentX < minX - 2f) // 왼쪽으로 너무 멀리 간 경우
            {
                _flyDirection = 1; // 오른쪽으로 강제 이동
            }
            else if (currentX > maxX + 2f) // 오른쪽으로 너무 멀리 간 경우
            {
                _flyDirection = -1; // 왼쪽으로 강제 이동
            }
            else
            {
                // 정상 범위 내에서의 경계 체크
                if (currentX <= minX && _flyDirection == -1)
                {
                    _flyDirection = 1; // 왼쪽 경계에서 오른쪽으로
                }
                else if (currentX >= maxX && _flyDirection == 1)
                {
                    _flyDirection = -1; // 오른쪽 경계에서 왼쪽으로
                }
            }
            
            // 좌우로 날아다니기
            Vector3 moveDirection = Vector3.right * _flyDirection;
            transform.position += moveDirection * flySpeed * Time.deltaTime;
            
            // Y 위치를 하늘 높이로 고정 (혹시 모를 Y축 이탈 방지)
            transform.position = new Vector3(transform.position.x, skyHeight, transform.position.z);
            
            // 다이브 타이머 감소
            _diveTimer -= Time.deltaTime;
            if (_diveTimer <= 0f)
            {
                StartDive();
            }
        }
        
        private void StartDive()
        {
            _currentState = BirdState.Warning;
            _targetPosition = new Vector3(transform.position.x, diveDepth, transform.position.z);
            
            // 경고 이펙트 생성
            if (warningEffect != null)
            {
                Instantiate(warningEffect, transform.position, Quaternion.identity);
            }
            
            StartCoroutine(DiveSequence());
        }
        
        private IEnumerator DiveSequence()
        {
            // 경고 시간 대기
            yield return new WaitForSeconds(diveWarningTime);
            
            // 다이브 시작
            _currentState = BirdState.Diving;
            _isDiving = true;
            
            // 다이브 이펙트 생성
            if (diveEffect != null)
            {
                Instantiate(diveEffect, transform.position, Quaternion.identity);
            }
        }
        
        private void HandleDiving()
        {
            // 빠르게 아래로 이동
            Vector3 direction = (_targetPosition - transform.position).normalized;
            transform.position += direction * diveSpeed * Time.deltaTime;
            
            // 목표 지점에 도달했는지 확인
            if (transform.position.y <= _targetPosition.y)
            {
                _currentState = BirdState.Holding;
                StartCoroutine(HoldAndReturn());
            }
        }
        
        private IEnumerator HoldAndReturn()
        {
            // 최하단에서 잠시 머무름
            yield return new WaitForSeconds(diveHoldTime);
            
            // 돌아가기 시작
            _currentState = BirdState.Returning;
            _isReturning = true;
        }
        
        private void HandleReturning()
        {
            // 플레이어 위치 기준으로 새로운 하늘 위치 계산
            if (_player != null)
            {
                float playerX = _player.transform.position.x;
                // 플레이어 주위에서 랜덤한 위치로 돌아가기
                float targetX = playerX + Random.Range(flyRangeMinX, flyRangeMaxX);
                _startPosition = new Vector3(targetX, skyHeight, transform.position.z);
            }
            
            // 새로운 시작 위치로 돌아가기
            Vector3 direction = (_startPosition - transform.position).normalized;
            transform.position += direction * diveSpeed * Time.deltaTime;
            
            // 시작 위치에 도달했는지 확인
            if (Vector3.Distance(transform.position, _startPosition) < 0.5f)
            {
                // 다시 날아다니기 모드로 전환
                _currentState = BirdState.Flying;
                _isDiving = false;
                _isReturning = false;
                _diveTimer = diveInterval; // 다이브 타이머 리셋
                
                transform.position = _startPosition;
            }
        }
        
        private void OnTriggerEnter2D(Collider2D collision)
        {
            // 다이브 중이거나 돌아가는 중일 때만 충돌 처리
            if (_currentState != BirdState.Diving && _currentState != BirdState.Returning)
                return;
            
            // 플레이어와 충돌
            if (collision.TryGetComponent<Player>(out Player player))
            {
                player.TakeDamage(damageToPlayer);
                Debug.Log($"중립새가 플레이어에게 {damageToPlayer} 데미지를 입혔습니다.");
            }
            
            // 적과 충돌
            if (collision.TryGetComponent<IDamageable>(out IDamageable enemy) && collision.GetComponent<Player>() == null)
            {
                enemy.TakeDamage(damageToEnemies);
                Debug.Log($"중립새가 적에게 {damageToEnemies} 데미지를 입혔습니다.");
            }
        }
        
        [Button("강제 다이브")]
        public void ForceDive()
        {
            if (_currentState == BirdState.Flying)
            {
                _diveTimer = 0f;
            }
        }
        
        [Button("상태 정보 출력")]
        public void PrintDebugInfo()
        {
            if (_player != null)
            {
                float playerX = _player.transform.position.x;
                float minX = playerX + flyRangeMinX;
                float maxX = playerX + flyRangeMaxX;
                float currentX = transform.position.x;
                
                Debug.Log($"새 상태: {_currentState}");
                Debug.Log($"플레이어 X: {playerX:F2}");
                Debug.Log($"새 X: {currentX:F2}");
                Debug.Log($"비행 범위: {minX:F2} ~ {maxX:F2}");
                Debug.Log($"비행 방향: {_flyDirection}");
                Debug.Log($"다이브 타이머: {_diveTimer:F2}");
            }
        }
        
        [Button("범위 내로 강제 이동")]
        public void ForceToRange()
        {
            if (_player != null && _currentState == BirdState.Flying)
            {
                float playerX = _player.transform.position.x;
                float targetX = playerX + Random.Range(flyRangeMinX, flyRangeMaxX);
                transform.position = new Vector3(targetX, skyHeight, transform.position.z);
                Debug.Log($"새를 범위 내로 이동시켰습니다: X = {targetX:F2}");
            }
        }
    }
} 