using System;

namespace AurumGames.SceneManagement
{
    /// <summary>
    /// Attribute to specify name of the scene
    /// </summary>
    public sealed class SceneInitScriptAttribute : Attribute
    {
        public string SceneName { get; }
        
        public SceneInitScriptAttribute(string name)
        {
            SceneName = name;
        }
    }
}