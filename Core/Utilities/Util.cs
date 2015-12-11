using System;
using System.Collections.Generic;
using System.Reflection;
using ExtensionMinder;

namespace WebMinder.Core.Utilities
{
    public static class Util
    {

        public static IList<string> GetFileExtensions()
        {
            return GetConstants(typeof(FileExtensions), true);
        }

        public static class FileExtensions
        {
            public static string Jpg = ".jpg";
            public static string Jpeg = ".jpeg";
            public static string Png = ".png";
            public static string Gif = ".gif";
            public static string Pdf = ".pdf";
            public static string Doc = ".doc";
            public static string Docx = ".docx";
            public static string Xls = ".xls";
            public static string Xlsx = ".xlsx";
            public static string Ppt = ".ppt";
            public static string Pptx = ".pptx";
            public static string Js = ".js";
            public static string Css = ".css";
            public static string Bmp = ".bmp";
            public static string Wmv = ".wmv";
            public static string Swf = ".swf";
        }

        public static IList<string> GetConstants(Type type, bool lowerToInvariantWithoutSpaces)
        {

            IList<string> list = new List<string>();

            FieldInfo[] fieldInfos = type.GetFields(
                // Gets all public and static fields

                BindingFlags.Public | BindingFlags.Static |
                // This tells it to get the fields from all base types as well

                BindingFlags.FlattenHierarchy);

            // Go through the list and only pick out the constants
            foreach (FieldInfo fi in fieldInfos)
                // IsLiteral determines if its value is written at 
                //   compile time and not changeable
                // IsInitOnly determine if the field can be set 
                //   in the body of the constructor
                // for C# a field which is readonly keyword would have both true 
                //   but a const field would have only IsLiteral equal to true
                if (fi.IsLiteral && !fi.IsInitOnly)
                {
                    string result = Convert.ToString(fi.GetValue(null));

                    list.Add(lowerToInvariantWithoutSpaces ? result.ToLowerInvariantWithOutSpaces() : result);
                }

            return list;
        }
    }
}
