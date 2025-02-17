using System;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UIElements;
using Yarn.Markup;
using Yarn.Unity;

public static class UIToolkitEffects
{
    /// <summary>
    /// A coroutine that gradually reveals the text in a <see
    /// cref="TextMeshProUGUI"/> object over time.
    /// </summary>
    /// <remarks>
    /// <para>This method works by adjusting the value of the <paramref name="text"/> parameter's <see cref="TextMeshProUGUI.maxVisibleCharacters"/> property. This means that word wrapping will not change half-way through the presentation of a word.</para>
    /// <para style="note">Depending on the value of <paramref name="lettersPerSecond"/>, <paramref name="onCharacterTyped"/> may be called multiple times per frame.</para>
    /// <para>Due to an internal implementation detail of TextMeshProUGUI, this method will always take at least one frame to execute, regardless of the length of the <paramref name="text"/> parameter's text.</para>
    /// </remarks>
    /// <param name="text">A TextMeshProUGUI object to reveal the text
    /// of.</param>
    /// <param name="lettersPerSecond">The number of letters that should be
    /// revealed per second.</param>
    /// <param name="onCharacterTyped">An <see cref="Action"/> that should be called for each character that was revealed.</param>
    /// <param name="stopToken">A <see cref="CoroutineInterruptToken"/> that
    /// can be used to interrupt the coroutine.</param>
    public static IEnumerator Typewriter(DialogueBoxController controller, float lettersPerSecond, Action onCharacterTyped, Effects.CoroutineInterruptToken stopToken = null)
    {
        stopToken?.Start();

        // Start with everything invisible.
        controller.SetVisibleCharacters(0);

        // Wait a single frame to let the text component process its
        // content, otherwise controller.Text.Length won't be
        // accurate.
        yield return null;

        // How many visible characters are present in the text?
        int characterCount = controller.Text.Length;

        // Early out if letter speed is zero, text length is zero
        if (lettersPerSecond <= 0 || characterCount == 0)
        {
            // Show everything and return
            controller.SetText(controller.Text);
            stopToken?.Complete();
            yield break;
        }

        // Convert 'letters per second' into its inverse
        float secondsPerLetter = 1.0f / lettersPerSecond;

        // If lettersPerSecond is larger than the average framerate, we
        // need to show more than one letter per frame, so simply
        // adding 1 letter every secondsPerLetter won't be good enough
        // (we'd cap out at 1 letter per frame, which could be slower
        // than the user requested.)
        //
        // Instead, we'll accumulate time every frame, and display as
        // many letters in that frame as we need to in order to achieve
        // the requested speed.
        float accumulator = Time.deltaTime;

        while (controller.VisibleCharacters < characterCount)
        {
            if (stopToken?.WasInterrupted ?? false)
                yield break;

            // Check for <alpha=#00> tag and remove it if found.
            if (controller.Text.Substring(controller.VisibleCharacters).StartsWith("<alpha=#00>"))
            {
                string beforeAlpha = controller.Text.Substring(0, controller.VisibleCharacters);
                string afterAlpha = controller.Text.Substring(controller.VisibleCharacters + "<alpha=#00>".Length);

                controller.SetText(beforeAlpha + afterAlpha, controller.VisibleCharacters);
                characterCount = controller.Text.Length;

                continue;  // Continue the loop without advancing the visible characters counter.
            }

            // Handle Rich Text tag parsing.
            (int tagLen, bool containsTargetAttribute) = RichTextTagSequenceLengthAt(controller.Text, controller.VisibleCharacters);

            // If the tag contains the target attribute, remove it from the text and continue processing.
            if (containsTargetAttribute)
            {
                string beforeTag = controller.Text.Substring(0, controller.VisibleCharacters);
                string tagContent = controller.Text.Substring(controller.VisibleCharacters, tagLen);
                string afterTag = controller.Text.Substring(controller.VisibleCharacters + tagLen);

                tagContent = RemoveInvisibleColorAttribute(tagContent);

                controller.SetText(beforeTag + tagContent + afterTag, controller.VisibleCharacters);
                characterCount = controller.Text.Length;
                tagLen = tagContent.Length;
            }

            // It's a tag sequence, skip the entire tag sequence and treat as non-existent.
            if (tagLen > 0)
                controller.SetVisibleCharacters(controller.VisibleCharacters + tagLen);
            // Normal character.
            else
                controller.SetVisibleCharacters(controller.VisibleCharacters + 1);

            onCharacterTyped?.Invoke();
            accumulator -= secondsPerLetter;

            // Wait for next character.
            while (accumulator < secondsPerLetter)
            {
                accumulator += Time.deltaTime;
                yield return null;
            }
        }

        // We either finished displaying everything, or were
        // interrupted. Either way, display everything now.
        controller.SetText(controller.Text);

        stopToken?.Complete();
    }

    /// <summary>
    /// Remove the invisible color attribute from the given string.
    /// </summary>
    private static string RemoveInvisibleColorAttribute(string tagContent)
    {
        const string targetAttribute = "color=#00000000";

        // Remove the target and clean up potential extra spaces
        if (tagContent.Contains(targetAttribute))
            return tagContent.Replace(targetAttribute, "").Replace("  ", " ").Trim();

        return tagContent;
    }

    /// <summary>
    /// Determine if a given position in a string starts a rich text tag sequence and
    /// return the length of that sequence if it does.
    /// </summary>
    private static (int, bool) RichTextTagSequenceLengthAt(string text, int index)
    {
        bool containsTargetAttribute = false;

        if (index < text.Length && text[index] == '<')
        {
            int closePos = text.IndexOf('>', index);

            if (closePos != -1)
            {
                string tagContent = text.Substring(index, closePos - index + 1);

                if (tagContent.Contains("color=#00000000"))
                    containsTargetAttribute = true;

                return (closePos - index + 1, containsTargetAttribute);
            }
        }

        return (0, containsTargetAttribute);
    }
}

[RequireComponent(typeof(UIDocument))]
public class UIToolkitLineView : DialogueViewBase
{
    // [SerializeField] private bool _showCharacterNameInLineView = true;
    [SerializeField] private bool _useTypewriterEffect = false;
    [SerializeField][Min(0)] private float _typewriterEffectSpeed = 0f;
    [Tooltip("How many characters typed it takes to emit the CharacterTyped event.")]
    [SerializeField][Range(1, 5)] private int _typeEmitFrequency = 1;
    [SerializeField] private float _holdTime = 1f;
    [SerializeField] private bool _autoAdvance = false;

    private LocalizedLine _currentLine;
    private Effects.CoroutineInterruptToken _currentStopToken = new Effects.CoroutineInterruptToken();
    private UIDocument _document;
    // private Coroutine _updateIconRoutine;

    public DialogueBoxController BoxController { get; private set; } = new();
    public DialogueViewController ViewController { get; private set; } = new();

    public event Action CharacterTyped;

    private void Awake()
    {
        _document = GetComponent<UIDocument>();
    }

    private void OnEnable()
    {
        ViewController.Initialize(
            _document.rootVisualElement.Q<VisualElement>(name = "dialogue-view"),
            ContinueButton_Clicked
        );
        BoxController.Initialize(
            _document.rootVisualElement.Q<DialogueBox>()
        );

        ViewController.ShowDialogueBox(false);

        // InputManager.ActionMapsChanged += InputManager_ActionMapsChanged;
        // InputManager.DeviceTypeChanged += InputManager_DeviceTypeChanged;
    }

    private void OnDisable()
    {
        // InputManager.ActionMapsChanged -= InputManager_ActionMapsChanged;
        // InputManager.DeviceTypeChanged -= InputManager_DeviceTypeChanged;
    }

    public override void DialogueComplete()
    {
        base.DialogueComplete();

        ViewController.ShowDialogueBox(false);
    }

    public override void InterruptLine(LocalizedLine dialogueLine, Action onInterruptLineFinished = null)
    {
        _currentLine = dialogueLine;

        // Cancel all coroutines that we're currently running. This will
        // stop the RunLineInternal coroutine, if it's running.
        StopAllCoroutines();

        // Show the entire line's text immediately.
        BoxController.SetText(ParseCustomMarkup(dialogueLine));
        BoxController.ShowConfirmation();

        onInterruptLineFinished?.Invoke();
    }

    public override void RunLine(LocalizedLine dialogueLine, Action onDialogueLineFinished)
    {
        // Stop any coroutines currently running on this line view
        // for example, any other RunLine that might be running
        StopAllCoroutines();

        // Begin running the line as a coroutine.
        StartCoroutine(RunLineInternal(dialogueLine, onDialogueLineFinished));
    }

    public override void UserRequestedViewAdvancement()
    {
        // We received a request to advance the view. If we're in the middle of
        // an animation, skip to the end of it. If we're not current in an
        // animation, interrupt the line so we can skip to the next one.

        // we have no line, so the user just mashed randomly
        if (_currentLine == null)
            return;

        // Is an animation running that we can stop?
        if (_currentStopToken.CanInterrupt)
        {
            // Stop the current animation, and skip to the end of whatever
            // started it.
            _currentStopToken.Interrupt();

            // Return so we require another request to advance to go to the next line.
            return;
        }

        // No animation is now running. Signal that we want to
        // interrupt the line instead.
        requestInterrupt?.Invoke();
    }

    public void ShowDialogueBox(bool show)
    {
        ViewController.ShowDialogueBox(show);
    }

    /// <summary>
    /// Parse custom markup for our dialogue.
    /// </summary>
    private string ParseCustomMarkup(LocalizedLine dialogueLine, bool shouldReveal = true)
    {
        string text = dialogueLine.Text.Text;

        // Sort attributes by position.
        dialogueLine.Text.Attributes.Sort((a, b) => a.Position.CompareTo(b.Position));

        // Loop backwards through attributes to avoid disrupting indices of unprocessed tags.
        for (int i = dialogueLine.Text.Attributes.Count - 1; i >= 0; i--)
        {
            MarkupAttribute attr = dialogueLine.Text.Attributes[i];

            if (attr.Name == "input")
            {
                MarkupValue action;
                attr.Properties.TryGetValue("action", out action);

                // Sprite icon = InputManager.GetBindingIconForAction(InputManager.FindAction(action.StringValue));

                // if (icon == null)
                //     continue;

                // string transparent = !shouldReveal ? "color=#00000000" : "";

                // text = text.Insert(attr.Position, $"<sprite=\"{icon.texture.name}\" name=\"{icon.name}\" {transparent}> ");
            }
        }

        // Handle <color> tags by injecting <alpha=#00> tags after the opening tag and after the closing tag.
        if (!shouldReveal)
        {
            // Add alpha tag after opening color tag:
            string pattern = @"(<color=(#?\w+|\""\w+\"")>)";
            string replacement = "$1<alpha=#00>";

            text = Regex.Replace(text, pattern, replacement);

            // Add alpha tag after closing color tag:
            text = text.Replace("</color>", "</color><alpha=#00>");
        }

        return text;
    }

    private IEnumerator RunLineInternal(LocalizedLine dialogueLine, Action onDialogueLineFinished)
    {
        string hiddenParsedText = ParseCustomMarkup(dialogueLine, false);
        string parsedText = ParseCustomMarkup(dialogueLine);

        IEnumerator PresentLine()
        {
            ViewController.ShowDialogueBox();

            // Hide the continue button until presentation is complete.
            BoxController.ShowConfirmation(false);

            if (_useTypewriterEffect)
            {
                // If we're using the typewriter effect, hide all of the
                // text before we begin any possible fade (so we don't fade
                // in on visible text).
                BoxController.SetText(hiddenParsedText, 0);

                // If we're using the typewriter effect, start it, and wait for
                // it to finish.
                int visibleCharacters = 0;

                yield return StartCoroutine(
                    UIToolkitEffects.Typewriter(
                        BoxController,
                        _typewriterEffectSpeed,
                        () => OnCharacterTyped(++visibleCharacters),
                        _currentStopToken
                    )
                );

                if (_currentStopToken.WasInterrupted)
                {
                    // The typewriter effect was interrupted. Stop this
                    // entire coroutine.
                    yield break;
                }
            }
        }

        _currentLine = dialogueLine;

        // Run any presentations as a single coroutine. If this is stopped,
        // which UserRequestedViewAdvancedment can do, then we will stop all
        // of the animations at once.
        yield return StartCoroutine(PresentLine());

        _currentStopToken.Complete();

        // All of our text should now be visible.
        BoxController.SetText(parsedText);

        // Show the continue button.
        BoxController.ShowConfirmation();

        // If we have a hold time, wait that amount of time, and then
        // continue.
        if (_holdTime > 0)
            yield return new WaitForSeconds(_holdTime);

        if (_autoAdvance == false)
        {
            // The line is now fully visible, and we've been asked to not
            // auto-advance to the next line. Stop here, and don't call the
            // completion handler - we'll wait for a call to
            // UserRequestedViewAdvancement, which will interrupt this
            // coroutine.
            yield break;
        }

        // Our presentation is complete; call the completion handler.
        onDialogueLineFinished();
    }

    /// <summary>
    /// Wait for the current animation to end, then rerender text to show the proper icon(s) for the device.
    /// </summary>
    // private IEnumerator UpdateInputSprites()
    // {
    //     while (!BoxController.ConfirmationShown)
    //         yield return null;

    //     BoxController.SetText(ParseCustomMarkup(_currentLine));
    //     _updateIconRoutine = null;
    // }

    #region Event Invokers

    private void OnCharacterTyped(int visibleCharacters)
    {
        if (visibleCharacters % _typeEmitFrequency == 0)
        {
            CharacterTyped?.Invoke();
        }
    }

    #endregion

    #region Event Handlers

    private void ContinueButton_Clicked()
    {
        // When the Continue button is clicked, we'll do the same thing as
        // if we'd received a signal from any other part of the game (for
        // example, if a DialogueAdvanceInput had signalled us.)
        UserRequestedViewAdvancement();
    }

    // private void InputManager_ActionMapsChanged(List<InputActionMap> actionMap)
    // {
    //     if (ViewController.Shown && _updateIconRoutine == null)
    //         _updateIconRoutine = StartCoroutine(UpdateInputSprites());
    // }

    // private void InputManager_DeviceTypeChanged(InputDevice device, InputControlScheme? controlScheme)
    // {
    //     if (ViewController.Shown && _updateIconRoutine == null)
    //         _updateIconRoutine = StartCoroutine(UpdateInputSprites());
    // }

    #endregion
}
