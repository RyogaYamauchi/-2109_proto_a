using System;
using System.Reflection;

namespace App.Lib
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class RootSceneNameAttribute : Attribute
    {
        public string Path { get; }

        public RootSceneNameAttribute(string path)
        {
            Path = path;
        }
    }

    public static class RootSceneName
    {
        public static string GetRootSceneName(Type type)
        {
            return type.GetCustomAttribute<RootSceneNameAttribute>().Path;
        }
    }
}