using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Resources;

namespace ReactReduxApi.Helpers
{
    public static class EnumsHelper
    {
        /// <summary>
        /// Gets the display name for an enum.
        /// </summary>
        /// <param name="enumValue"></param>
        /// <exception cref="ArgumentException"></exception>
        /// <returns></returns>
        public static string GetDisplayName(this Enum enumValue)
        {
            var enumType = enumValue.GetType();
            var names = new List<string>();
            foreach (var e in Enum.GetValues(enumType))
            {
                var flag = (Enum)e;
                if (enumValue.HasFlag(flag))
                {
                    names.Add(GetSingleDisplayName(flag));
                }
            }
            if (names.Count <= 0) throw new ArgumentException("Names list is empty");
            if (names.Count == 1) return names.First();
            return string.Join(",", names);
        }

        /// <summary>
        /// Gets the display value for a single enum flag or 
        /// name of that flag if the display value is not set
        /// </summary>
        /// <param name="flag"></param>
        /// <returns></returns>
        public static string GetSingleDisplayName(this Enum flag)
        {
            try
            {
                
                var attr = flag.GetType()
                        .GetMember(flag.ToString())
                        .First()
                        .GetCustomAttribute<DisplayAttribute>();
                return attr.ResourceType == null ? attr.Name : (new ResourceManager(attr.ResourceType.FullName, attr.ResourceType.Assembly)).GetString(attr.Name); 

            }
            catch
            {
                return flag.ToString();
            }
        }
    }
}
