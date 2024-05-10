using System.ComponentModel;

namespace SurveyUs.Domain.Enums
{
    public enum QuestionTypeEnum
    {
        [Description("True/False")]
        TrueFalse = 1,
        [Description("Multiple Choice")]
        MultipleChoice,
        [Description("Multi Select")]
        MultiSelect,
        [Description("Points")]
        PointBased,
        [Description("Short Answer")]
        ShortAnswer
    }
}
