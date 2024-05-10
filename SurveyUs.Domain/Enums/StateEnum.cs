using System.ComponentModel;

namespace SurveyUs.Domain.Enums
{
    public enum StateEnum
    {
        [Description("Johor")]
        Johor = 1,
        [Description("Kedah")]
        Kedah,
        [Description("Kelantan")]
        Kelantan,
        [Description("Melaka")]
        Melaka,
        [Description("Negeri Sembilan")]
        NegeriSembilan,
        [Description("Pahang")]
        Pahang,
        [Description("Pulau Pinang")]
        PulauPinang,
        [Description("Perak")]
        Perak,
        [Description("Perlis")]
        Perlis,
        [Description("Selangor")]
        Selangor,
        [Description("Terengganu")]
        Terengganu,
        [Description("Sabah")]
        Sabah,
        [Description("Sarawak")]
        Sarawak,
        [Description("Wilayah Persekutuan Kuala Lumpur")]
        WPKualaLumpur,
        [Description("Wilayah Persekutuan Labuan")]
        WPLabuan,
        [Description("Wilayah Persekutuan Putrajaya")]
        WPPutrajaya
    }
}
