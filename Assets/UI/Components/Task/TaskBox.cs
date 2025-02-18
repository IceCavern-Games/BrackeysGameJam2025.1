using UnityEngine.UIElements;

[UxmlElement]
public partial class TaskBox : VisualElement
{
    #region UXML Attribute Getter/Setters

    [UxmlAttribute]
    public string TaskName
    {
        get => _taskName.text;
        private set => SetTaskName(value);
    }

    [UxmlAttribute]
    public string TaskDescription
    {
        get => _taskDescription.text;
        private set => SetTaskDescription(value);
    }
    
    #endregion
    #region UI Element References

    private readonly Label _taskName;
    private readonly Label _taskDescription;

    #endregion

    public TaskBox()
    {
        _taskName = new Label { name = "TaskBox__taskName" };
        Add(_taskName);

        _taskDescription = new Label { name = "TaskBox__taskDescription" };
        
        Add(_taskDescription);
    }

    public void SetTaskName(string text)
    {
        _taskName.text = text;
    }

    public void SetTaskDescription(string text)
    {
        _taskDescription.text = text;
    }
}
