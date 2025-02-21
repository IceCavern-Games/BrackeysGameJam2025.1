using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

[UxmlElement]
public partial class CarouselField : SelectorField<string>
{
    public CarouselField() : this(null) { }

    public CarouselField(string label) : base(label) { }

    public CarouselField(List<string> choices, string defaultValue, Func<string, string> formatSelectedValueCallback = null, Func<string, string> formatListItemCallback = null)
        : this(null, choices, defaultValue, formatSelectedValueCallback, formatListItemCallback) { }

    public CarouselField(string label, List<string> choices, string defaultValue, Func<string, string> formatSelectedValueCallback = null, Func<string, string> formatListItemCallback = null)
        : base(label, choices, defaultValue, formatSelectedValueCallback, formatListItemCallback) { }

    public CarouselField(List<string> choices, int defaultIndex, Func<string, string> formatSelectedValueCallback = null, Func<string, string> formatListItemCallback = null)
            : this(null, choices, defaultIndex, formatSelectedValueCallback, formatListItemCallback) { }

    public CarouselField(string label, List<string> choices, int defaultIndex, Func<string, string> formatSelectedValueCallback = null, Func<string, string> formatListItemCallback = null)
        : base(label, choices, defaultIndex, formatSelectedValueCallback, formatListItemCallback) { }
}
