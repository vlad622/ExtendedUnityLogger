using System.Reflection;
using UnityEngine;

namespace VRP.Helpers
{
    public static class StringHelper
    {
        public static string GetColoredString(this string s, Color color)
        {
            string colorName = "";
            PropertyInfo[] props = color.GetType().GetProperties(BindingFlags.Public | BindingFlags.Static);

            foreach (PropertyInfo prop in props)
            {
                if ((Color) prop.GetValue(null, null) == color)
                {
                    colorName = prop.Name;
                }
            }

            if (colorName == "") colorName = color.ToString();

            return $"<color={colorName}> {s} </color>";
        }

        public static string GetBoldString(this string s)
        {
            return $"<b= {s} </b>";
        }
    }
}