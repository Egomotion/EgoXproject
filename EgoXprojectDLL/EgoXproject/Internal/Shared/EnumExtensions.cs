//------------------------------------------
//  EgoXproject
//  Copyright © 2013-2019 Egomotion Limited
//------------------------------------------

//http://cyotek.com/blog/using-alternate-descriptions-for-enumeration-members
using System;
using System.ComponentModel;
using System.Reflection;


namespace Egomotion.EgoXproject.Internal
{
    internal static class EnumExtensions
    {
        //Get a description for a value
        public static string GetDescription<T>(this T value)
        where T : struct
        {
            FieldInfo field = value.GetType().GetField(value.ToString());
            DescriptionAttribute attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;

            return attribute != null ? attribute.Description : string.Empty;
        }

        //get a value for a description
        public static T GetValueWithDescription<T>(string value, T defaultValue)
        {
            foreach (T id in Enum.GetValues(typeof(T)))
            {
                FieldInfo field = id.GetType().GetField(id.ToString());
                DescriptionAttribute attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;

                if (attribute != null && attribute.Description == value)
                {
                    return id;
                }
            }

            return defaultValue;
        }

        public static string GetXcodeDataValue<T>(this T value)
        where T : struct
        {
            FieldInfo field = value.GetType().GetField(value.ToString());
            XcodeDataValueAttribute attribute = Attribute.GetCustomAttribute(field, typeof(XcodeDataValueAttribute)) as XcodeDataValueAttribute;

            return attribute != null ? attribute.Value : string.Empty;
        }

    }

}