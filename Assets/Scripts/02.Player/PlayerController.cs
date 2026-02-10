using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.PlayerLoop;
using UnityEngine.Timeline;

public class PlayerController : MonoBehaviour
{
    private Animator _playerAnimator;
    private Rigidbody2D _rb;
    public Vector3 orientation { get; private set; }

    [SerializeField] private float _playerSpeed;
    private int _maxHp = 10;
    public ObservableProperty<int> PlayerHP;
    public bool isDead;
    
    public BulletLauncher launcher;

    [HideInInspector] public UnityEvent OnPlayerDeath; 
    [HideInInspector] public UnityEvent OnPlayerWin; 

    
    private void Awake() => Init();

    private void OnEnable()
    {
        GameManager.Instance.player = this;
        PlayerHP.Value = 10;
        PlayerHP.Subscribe(CheckPlayerHP);
    }
    
    private void OnDisable()
    {
        GameManager.Instance.player = null;
        PlayerHP.Unsubscribe(CheckPlayerHP);
    }

    private void Update()
    {
        SetMove();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var pickupable = collision.GetComponent<IPickupable>();
        
        if (pickupable != null)
        {
            // 스킬, 열쇠, 아이템 한번에 처리
            pickupable.OnPickup();
        }
    }

    private void SetMove()
    {
        if (isDead)
        {
            return;
        }
        Vector3 dir = GetNormalizedDirection();

        if (dir == Vector3.zero)
        {   
            return;
        }
        
        orientation = dir; // Input이 있을 때만 플레이어의 방향 갱신

        _rb.velocity =  dir * _playerSpeed;
        
        _playerAnimator.SetFloat("Speed", _rb.velocity.magnitude);
        _playerAnimator.SetFloat("MoveX", _rb.velocity.x);
        _playerAnimator.SetFloat("MoveY", _rb.velocity.y);
    }

    private Vector3 GetNormalizedDirection()
    {
        Vector3 dir = Vector3.zero;
        dir = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"),0).normalized;

        return dir;
    }
    
    public void TakeDamage(int damage)  // Monster에서 호출됨
    {
        PlayerHP.Value -= damage;
        _playerAnimator.SetTrigger("Hit");
    }

    private void CheckPlayerHP(int hp)
    {
        if (PlayerHP.Value <= 0)
        {
            PlayerCharacterDie();
        }
        
        // TODO: 체력 변화에 따른 이벤트가 있다면 여기서 추가하면 됨
        // Ex> 체력이 가득차면 특정 효과를 켠다던지
        //      체력이 50%이하로 떨어지면 힘들어하는 이펙트를 켠다던지
    }

    private void PlayerCharacterDie()
    {
        isDead = true; // 이동 및 공격 입력을 그만 받기 위한 변수
        _rb.constraints = RigidbodyConstraints2D.FreezeAll; // 현재 위치에 플레이어 고정
        _playerAnimator.SetBool("Die", true); // 사망에 따른 애니메이션
        SoundManager.Instance.StopBGM();
        SoundManager.Instance.PlaySFX(ESFXs.DeathSFX);
        Invoke("OpenGameOver", 1.5f);
        Cursor.visible = true;
    }
    
    private void Init()
    {
        _rb = GetComponent<Rigidbody2D>();
        _playerAnimator = GetComponent<Animator>();
        orientation = new Vector3(0, -1, 0);
        PlayerHP = new ObservableProperty<int>(_maxHp);
    }

    private void OpenGameOver()
    {
        Time.timeScale = 0;
        OnPlayerDeath?.Invoke(); // 블러처리 및 팝업 열기
        SoundManager.Instance.PlaySFX(ESFXs.LoseSFX);
    }
}
