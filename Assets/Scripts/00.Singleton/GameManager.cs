using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public StageDataSetter setter;
    public MapGenerator generator;
    public Spawner spawner;
    public CamController camController;

    public PlayerController player;
    
    void Awake() => Init();

    private void Init()
    {
        SingletonInit();
        setter = GetComponentInChildren<StageDataSetter>();
    }

    private void SingletonInit()
    {
        if (Instance != null)
        { 
            Destroy(gameObject);
        }

        else if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void Reset() // 테스트용 임시함수
    {
        setter.stageRoomList = null;
        setter.stageRoomList = new();
        spawner.DestroyMonsters();
        spawner.DestroyPlayerAndGoal();
    }

    public void StartGame()
    {
        SceneManager.LoadScene(1);
        // fake loading
        // StartCoroutine(Init());
    }

    public void RestartGame()
    {
        Reset();
        SceneManager.LoadScene(1);
    }

    public void LoadScene(int sceneNumber)
    {
        SceneManager.LoadScene(sceneNumber);
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

}
