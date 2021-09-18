using System;
using System.Reflection;

namespace App.Lib
{
    [AttributeUsage(AttributeTargets.Class)]
    public class PrefabPathAttribute : Attribute
    {
        public string Path { get; }

        public PrefabPathAttribute(string path)
        {
            Path = path;
        }
    }

    public static class PrefabPath
    {
        public static string GetPrefabPath(Type type)
        {
            return type.GetCustomAttribute<PrefabPathAttribute>().Path;
        }
    }
}