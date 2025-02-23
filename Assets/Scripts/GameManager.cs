using Reflex.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class BadgeData
{
    public string Attempts = "0000000";
    public string Name = "Mark S.";
}

[RequireComponent(typeof(UIDocument))]
public class GameManager : MonoBehaviour
{
    public BadgeData BadgeData { get; private set; } = new();
    public Timer Clock { get; private set; }
    public string ClockTime => TimeUtils.ElapsedTimeToDisplay(Clock.ElapsedTime);

    private VisualElement _fade;
    private float _fadeDuration = 1;
    private Action _transitionCallback;
    private UIDocument _uiDocument;

    [Inject] private readonly DialogueManager _dialogueManager;
    [Inject] private readonly InputManager _inputManager;
    [Inject] private readonly PaintTextureManager _paintTextureManager;
    [Inject] private readonly TaskManager _taskManager;

    private int _attempts = 0;
    private RandomNameGenerator _nameGenerator;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        _uiDocument = GetComponent<UIDocument>();

        Clock = new Timer(510, false); // Will tick for 8.5 in-game hours.
        _nameGenerator = new();
    }

    private void OnEnable()
    {
        _fade = _uiDocument.rootVisualElement.Q<VisualElement>("screen-fade");
    }

    private void Update()
    {
        Clock.Tick(Time.deltaTime);
    }

    public void EndAttempt()
    {
        Clock.Stop();
        Clock.Reset();
        _taskManager.Reset();

        _inputManager.Input.enabled = false;
        FadeOut(_fadeDuration, () =>
        {
            StartCoroutine(LoadScene("TheOffice", () =>
            {
                _inputManager.Input.enabled = true;
            }));
        });
    }

    public void ExitGame()
    {
        FadeOut(_fadeDuration, () =>
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
            Application.Quit();
        });
    }

    public void Fail()
    {
        _dialogueManager.StartConversation("Fail", () =>
        {
            EndAttempt();
        });
    }

    public void LoadTheOffice()
    {
        Clock.Stop();
        Clock.Reset();
        _taskManager.Reset();
        _paintTextureManager.Clear();
        _attempts = 0;

        FadeOut(_fadeDuration, () =>
        {
            StartCoroutine(LoadScene("TheOffice", () => { }));
        });
    }

    public void LoadTitleScreen()
    {
        FadeOut(_fadeDuration, () =>
        {
            StartCoroutine(LoadScene("TitleScreen", () =>
            {
                _inputManager.SetCursorLock(false);
            }));
        });
    }

    public void StartAttempt()
    {
        UpdateBadge();

        _dialogueManager.StartConversation("Intro", () =>
        {
            Clock.Start();
        });
    }

    public void Win()
    {
        _dialogueManager.StartConversation("Win", () =>
        {
            LoadTitleScreen();
        });
    }

    private void FadeIn(float duration, Action callback = null)
    {
        _fade.style.transitionDuration = new List<TimeValue> { duration };

        if (callback != null)
        {
            _transitionCallback = callback;
            _fade.RegisterCallback<TransitionEndEvent>(FadeTransition_End);
        }

        _fade.AddToClassList("hidden");
    }

    private void FadeOut(float duration, Action callback = null)
    {
        _fade.style.transitionDuration = new List<TimeValue> { duration };

        if (callback != null)
        {
            _transitionCallback = callback;
            _fade.RegisterCallback<TransitionEndEvent>(FadeTransition_End);
        }

        _fade.RemoveFromClassList("hidden");
    }

    private void FadeTransition_End(TransitionEndEvent e)
    {
        _fade.UnregisterCallback<TransitionEndEvent>(FadeTransition_End);
        _transitionCallback?.Invoke();
        _transitionCallback = null;
    }

    private IEnumerator LoadScene(string sceneName, Action callback)
    {
        yield return null; // wait a frame

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;

        // Wait until the asynchronous scene fully loads.
        while (!asyncLoad.isDone)
        {
            Debug.Log($"Loading Progress: {asyncLoad.progress * 100}%");

            if (asyncLoad.progress >= 0.9f)
                asyncLoad.allowSceneActivation = true;

            yield return null;
        }

        FadeIn(_fadeDuration, () =>
        {
            callback();
        });
    }

    private void UpdateBadge()
    {
        _attempts++;

        BadgeData.Name = _nameGenerator.GetRandomName();
        BadgeData.Attempts = _attempts.ToString("D6");
    }
}
