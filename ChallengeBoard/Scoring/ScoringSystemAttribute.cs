using System;

namespace ChallengeBoard.Scoring
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class ScoringSystemAttribute : Attribute
    {
        public string SystemName { get; private set; }
        public string Description { get; set; }

        public ScoringSystemAttribute(string systemName, string description)
        {
            SystemName = systemName;
            Description = description;
        }
    }
}