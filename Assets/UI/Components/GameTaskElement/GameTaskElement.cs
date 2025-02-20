using Unity.Properties;
using UnityEngine.UIElements;

[UxmlElement]
public partial class GameTaskElement : VisualElement
{
    private readonly Label _taskName;
    private readonly Label _taskDescription;
    private readonly Label _taskDeadline;

    public GameTaskElement()
    {
        _taskName = new Label { name = "name" };
        _taskName.SetBinding("text", new DataBinding
        {
            dataSource = dataSource,
            dataSourcePath = new PropertyPath(nameof(GameTask.Name)),
            bindingMode = BindingMode.ToTarget
        });

        _taskDescription = new Label { name = "description" };
        _taskDescription.SetBinding("text", new DataBinding
        {
            dataSourcePath = new PropertyPath(nameof(GameTask.Description)),
            bindingMode = BindingMode.ToTarget
        });

        _taskDeadline = new Label { name = "deadline" };
        var deadlineBinding = new DataBinding
        {
            dataSourcePath = new PropertyPath(nameof(GameTask.Deadline)),
            bindingMode = BindingMode.ToTarget
        };
        deadlineBinding.sourceToUiConverters.AddConverter<int, string>(ConvertIntToTimeString);
        _taskDeadline.SetBinding("text", deadlineBinding);

        Add(_taskName);
        Add(_taskDescription);
        Add(_taskDeadline);
    }

    private string ConvertIntToTimeString(ref int value) => $"Deadline: {TimeUtils.ElapsedTimeToDisplay(value)}";
}
