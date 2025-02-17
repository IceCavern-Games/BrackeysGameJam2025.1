using Reflex.Attributes;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Yarn.Unity;

public class DialogueManager : MonoBehaviour
{
    [Inject] private readonly AudioManager _audioManager;
    // [Inject] private readonly SaveManager _saveManager;

    [SerializeField] private AudioClip _defaultDialogueSFX;

    private DialogueRunner _dialogueRunner;
    private UIToolkitLineView _dialogueView;
    private VariableStorageBehaviour _storage;

    private UnityAction _currentConversationCallback;
    private AudioClip _dialogueSFX;
    private List<string> _queuedTriggerActivations = new();

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        _dialogueRunner = GetComponent<DialogueRunner>();
        _dialogueView = GetComponentInChildren<UIToolkitLineView>();
        _storage = GetComponent<VariableStorageBehaviour>();

        _dialogueRunner.onDialogueComplete.AddListener(DialogueRunner_DialogueCompleted);

        _dialogueSFX = _defaultDialogueSFX;
    }

    private void OnEnable()
    {
        _dialogueView.CharacterTyped += DialogueView_CharacterTyped;
    }

    private void OnDisable()
    {
        _dialogueView.CharacterTyped -= DialogueView_CharacterTyped;
    }

    /// <summary>
    /// Clear variable storage (for things like resetting visit count).
    /// </summary>
    public void ClearStorage()
    {
        _storage.Clear();
    }

    /// <summary>
    /// Queue a trigger to be activated once dialogue finishes.
    /// </summary>
    public void QueueTrigger(string triggerID)
    {
        _queuedTriggerActivations.Add(triggerID);
    }

    /// <summary>
    /// Temporarily show/hide the dialogue box.
    /// </summary>
    public void ShowDialogueBox(bool show = true)
    {
        _dialogueView.ShowDialogueBox(show);
    }

    /// <summary>
    /// Start running some dialogue.
    /// </summary>
    public void StartConversation(string startNode, UnityAction callback = null, AudioClip dialogueSFX = null)
    {
        if (callback != null)
        {
            _currentConversationCallback = callback;
            _dialogueRunner.onDialogueComplete.AddListener(_currentConversationCallback);
        }

        if (dialogueSFX != null)
            _dialogueSFX = dialogueSFX;

        _dialogueRunner.StartDialogue(startNode);
    }

    /// <summary>
    /// Force the currently running dialogue to end.
    /// </summary>
    public void StopConversation()
    {
        _dialogueRunner.Stop();
    }

    #region Event Handlers

    private void DialogueRunner_DialogueCompleted()
    {
        if (_currentConversationCallback != null)
        {
            _dialogueRunner.onDialogueComplete.RemoveListener(_currentConversationCallback);
            _currentConversationCallback = null;
        }

        _dialogueSFX = _defaultDialogueSFX;

        // foreach (string triggerID in _queuedTriggerActivations)
        //     _saveManager.Scene.SetTrigger(triggerID);

        _queuedTriggerActivations.Clear();
    }

    private void DialogueView_CharacterTyped()
    {
        _audioManager.PlayDialogue(_dialogueSFX);
    }

    #endregion
}
