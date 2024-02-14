using System;
using JetBrains.Annotations;

namespace AurumGames.CompositeRoot
{
    /// <summary>
    /// Dependency attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Field)]
    [MeansImplicitUse(ImplicitUseKindFlags.Assign)]
    public sealed class DependencyAttribute : Attribute
    {
        /// <summary>
        /// Resolve priority
        /// Greater = later
        /// </summary>
        public int Priority { get; }
        /// <summary>
        /// False to mark as optional
        /// </summary>
        public bool Required { get; set; } = true;
        
        public DependencyAttribute(int priority = -1)
        {
            Priority = priority;
        }
    }
}