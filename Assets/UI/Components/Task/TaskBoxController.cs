public class TaskBoxController
{
    #region UI Element References

    private TaskBox _taskBox;
    
    #endregion

    public string TaskName { get; private set; } = "";
    public string TaskDescription { get; private set; } = "";

    public void Initialize(TaskBox visualElement)
    {
        _taskBox = visualElement;
        
        RenderTaskName();
        RenderTaskDescription();
    }

    /// <summary>
    /// Set the text to be displayed in the task name label
    /// </summary>
    /// <param name="text"></param>
    public void SetTaskName(string text)
    {
        TaskName = text;
        RenderTaskName();
    }

    /// <summary>
    /// Set the text to be displayed in the task description label
    /// </summary>
    /// <param name="text"></param>
    public void SetTaskDescription(string text)
    {
        TaskDescription = text;
        RenderTaskDescription();
    }
    
    /// <summary>
    /// Display the name text
    /// </summary>
    private void RenderTaskName()
    {
        _taskBox.SetTaskName(TaskName);
    }

    /// <summary>
    /// Display the description text
    /// </summary>
    private void RenderTaskDescription()
    {
        _taskBox.SetTaskDescription(TaskDescription);
    }
}
