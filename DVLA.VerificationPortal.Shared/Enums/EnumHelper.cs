using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.VerificationPortal.Shared.Enums
{
    public static class EnumHelper
    {
        public static string GetEnumDescription(Enum enumVal)
        {
            FieldInfo fieldInfo=enumVal.GetType().GetField(enumVal.ToString());
            DescriptionAttribute descriptionAttribute=fieldInfo?.GetCustomAttribute<DescriptionAttribute>();

            return descriptionAttribute?.Description ?? enumVal.ToString();
        }
    }
}
