using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.VerificationPortal.Infrastructure.Enums
{
    public static class EnumHelper
    {
        public static string GetEnumDescription(Enum enumVal)
        {
            FieldInfo fieldInfo = enumVal.GetType().GetField(enumVal.ToString());
            DescriptionAttribute descriptionAttribute = fieldInfo?.GetCustomAttribute<DescriptionAttribute>();

            return descriptionAttribute?.Description ?? enumVal.ToString();
        }

        public static List<T> EnumToList<T>() where T : Enum
        {
            return Enum.GetValues(typeof(T)).Cast<T>().ToList();
        }

        public static List<KeyValuePair<int, string>> EnumToListWithDescription<T>() where T : Enum
        {
            return Enum.GetValues(typeof(T))
                       .Cast<T>()
                       .Select(e => new KeyValuePair<int, string>(
                           Convert.ToInt32(e),
                           GetEnumDescription(e)
                       ))
                       .ToList();
        }

        private static string GetEnumDescription<T>(T enumVal) where T : Enum
        {
            FieldInfo fieldInfo = typeof(T).GetField(enumVal.ToString());
            var descriptionAttribute = fieldInfo?.GetCustomAttribute<DescriptionAttribute>();
            return descriptionAttribute?.Description ?? enumVal.ToString();
        }
    }
}
