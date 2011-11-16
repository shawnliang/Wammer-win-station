namespace Waveface.Component.HtmlEditor
{
    #region HtmlListType

    // Enum used to insert a list
    public enum HtmlListType
    {
        Ordered,
        Unordered
    }

    #endregion

    #region HtmlHeadingType

    // Enum used to insert a heading
    public enum HtmlHeadingType
    {
        H1 = 1,
        H2 = 2,
        H3 = 3,
        H4 = 4,
        H5 = 5
    }

    #endregion

    #region NavigateActionOption

    // Enum used to define the navigate action on a user clicking a href
    public enum NavigateActionOption
    {
        Default,
        NewWindow,
        SameWindow
    }

    #endregion

    #region ImageAlignOption

    // Enum used to define the image align property
    public enum ImageAlignOption
    {
        AbsBottom,
        AbsMiddle,
        Baseline,
        Bottom,
        Left,
        Middle,
        Right,
        TextTop,
        Top
    }

    #endregion

    #region HorizontalAlignOption

    // Enum used to define the text alignment property
    public enum HorizontalAlignOption
    {
        Default,
        Left,
        Center,
        Right
    }

    #endregion

    #region VerticalAlignOption

    // Enum used to define the vertical alignment property
    public enum VerticalAlignOption
    {
        Default,
        Top,
        Bottom
    }

    #endregion

    #region DisplayScrollBarOption

    // Enum used to define the visibility of the scroll bars
    public enum DisplayScrollBarOption
    {
        Yes,
        No,
        Auto
    }

    #endregion

    #region MeasurementOption

    // Enum used to define the unit of measure for a value
    public enum MeasurementOption
    {
        Pixel,
        Percent
    }

    #endregion
}