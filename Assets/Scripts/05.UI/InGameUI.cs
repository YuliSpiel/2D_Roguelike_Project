using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class InGameUI : MonoBehaviour, IInitializable
{
    [SerializeField] private TextMeshProUGUI _monsterCount;
    [SerializeField] private TextMeshProUGUI _keyCount;

    [SerializeField] private StageDataSetter _stageDataSetter;
    [SerializeField] private LifeUI _lifeUI;
    
    [SerializeField] private Canvas _gameOverCanvas;
    [SerializeField] private Canvas _winCanvas;

    // 플레이어 체력
    private PlayerController _player;
    [SerializeField] private Camera _mainCamera;

    [SerializeField] private GameObject _mouseAim; // 에임 표시

    // 남은 열쇠 팝업 : 열쇠를 다 못 찾고 골인 지점에 도달했을 때 띄울 팝업
    [SerializeField] private GameObject _keyCountPopup;
    [SerializeField] private TextMeshProUGUI _leftKeyCountText;
    
    private void Awake()
    {
        _gameOverCanvas.gameObject.SetActive(false);
        _winCanvas.gameObject.SetActive(false);
        _keyCountPopup.SetActive(false);
    }

    private void OnEnable()
    {
        SubscribeEvents();
    }

    private void OnDisable() => UnsubscribeEvents();


    private void Update()
    {
        SetAimUI();
    }

    private void SetAimUI()
    {
        Vector3 mouseWorldPos = Input.mousePosition;
        mouseWorldPos.z = 0;
        _mouseAim.transform.position = mouseWorldPos;
    }

    public void SceneInitialize()
    {
        Debug.Log("InGameUI : Scene Initialize");

        Cursor.visible = false;
        _stageDataSetter = GameManager.Instance.setter;
        _player = GameManager.Instance.player;
        SetMonsterCountUI(_stageDataSetter.MonsterCount.Value);
        SetKeyCountUI(_stageDataSetter.KeyCount.Value);

        SubscribeEvents();
    }

    private void SubscribeEvents()
    {
        if (_stageDataSetter != null)
        {
            _stageDataSetter.MonsterCount.Subscribe(SetMonsterCountUI);
            _stageDataSetter.KeyCount.Subscribe(SetKeyCountUI);
        }

        if (_player != null)
        {
            _player.PlayerHP.Subscribe(_lifeUI.OnPlayerHPChanged);
            _player.OnPlayerDeath.AddListener(OpenGameOverCanvas);
            _player.OnPlayerWin.AddListener(OpenWinCanvas);
        }
    }

    private void UnsubscribeEvents()
    {
        _stageDataSetter.MonsterCount.Unsubscribe(SetMonsterCountUI);
        _stageDataSetter.KeyCount.Unsubscribe(SetKeyCountUI);
        _player.PlayerHP.Unsubscribe(_lifeUI.OnPlayerHPChanged);
        _player.OnPlayerDeath.RemoveListener(OpenGameOverCanvas);
        _player.OnPlayerWin.RemoveListener(OpenWinCanvas);
    }
    
    public void SetMonsterCountUI(int value)
    {
        _monsterCount.text = value.ToString();
    }

    public void SetKeyCountUI(int value)
    {
        _keyCount.text = value.ToString();
    }

    public void OpenGameOverCanvas()
    {
        _gameOverCanvas.gameObject.SetActive(true);
    }

    public void OpenWinCanvas()
    {
        _winCanvas.gameObject.SetActive(true);
    }

    public void OpenTitleScene()
    {
        SoundManager.Instance.StopSFX();
        GameManager.Instance.LoadScene(0);
        Time.timeScale = 1;
    }

    public void ShowLeftKeyCount()
    {
        _leftKeyCountText.text = GameManager.Instance.setter.KeyCount.Value.ToString();
        _keyCountPopup.SetActive(true);
        Invoke("HideKeyCountPopup", 2f);
    }

    private void HideKeyCountPopup()
    {
        _keyCountPopup.SetActive(false);
    }
    
    public void RetryGame()
    {
        SoundManager.Instance.StopSFX();
        GameManager.Instance.RestartGame();
        Time.timeScale = 1;
    }

}
