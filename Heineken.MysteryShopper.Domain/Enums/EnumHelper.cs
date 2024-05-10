using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text.Json;

namespace SurveyUs.Domain.Enums
{
    public class EnumDropDownListModel
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }

    public static class EnumHelper
    {
        public static string GetDescription<T>(this T e) where T : IConvertible
        {
            if (e is Enum)
            {
                Type type = e.GetType();
                Array values = System.Enum.GetValues(type);

                foreach (int val in values)
                {
                    if (val == e.ToInt32(CultureInfo.InvariantCulture))
                    {
                        var memInfo = type.GetMember(type.GetEnumName(val));
                        var descriptionAttribute = memInfo[0]
                            .GetCustomAttributes(typeof(DescriptionAttribute), false)
                            .FirstOrDefault() as DescriptionAttribute;

                        if (descriptionAttribute != null)
                        {
                            return descriptionAttribute.Description;
                        }
                    }
                }
            }

            return null; // could also return string.Empty
        }

        public static T GetValueFromDescription<T>(string description) where T : Enum
        {
            foreach (var field in typeof(T).GetFields())
            {
                if (Attribute.GetCustomAttribute(field,
                typeof(DescriptionAttribute)) is DescriptionAttribute attribute)
                {
                    if (attribute.Description == description)
                        return (T)field.GetValue(null);
                }
                else
                {
                    if (field.Name == description)
                        return (T)field.GetValue(null);
                }
            }

            throw new ArgumentException("Not found.", nameof(description));
            // Or return default(T);
        }

        public static List<EnumDropDownListModel> ToSelectionList<T>(bool isLocalized = true) where T : struct, Enum
        {
            //if (!typeof(TEnum).GetTypeInfo().IsEnum)
            //    throw new ArgumentException("Enumeration type is required.");

            var array = (T[])(Enum.GetValues(typeof(T)).Cast<T>());
            return array.Select(a => new EnumDropDownListModel
            {
                Name = isLocalized == true ? a.GetDescription() : a.GetDescription(),
                Id = Convert.ToInt32(a)
            })
                .OrderBy(x => x.Id)
                .ToList();
        }

        //public static string LocalizedEnum(this string desc)
        //{
        //    var localizationSource = LocalizationHelper.GetSource(ConfianceConsts.LocalizationSourceName);

        //    return localizationSource.GetString(desc);
        //}

        public static string GetEnumAsJson<T>()
        {
            var enumValues = Enum.GetValues(typeof(T)).Cast<T>();
            var enumDictionary = new Dictionary<int, string>();
            foreach (var value in enumValues)
            {
                enumDictionary[(int)Convert.ChangeType(value, typeof(int))] = value.ToString();
            }
            return JsonSerializer.Serialize(enumDictionary);
        }
    }
}
