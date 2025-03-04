using System;
using System.Collections.Generic;

namespace OuiAI.Common.Models
{
    public class Project
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ThumbnailUrl { get; set; }
        public Guid OwnerId { get; set; }
        public string OwnerUsername { get; set; }
        public ProjectCategory Category { get; set; }
        public List<string> Tags { get; set; } = new List<string>();
        public int LikesCount { get; set; }
        public int ViewsCount { get; set; }
        public int CommentsCount { get; set; }
        public ProjectVisibility Visibility { get; set; }
        public List<string> MediaUrls { get; set; } = new List<string>();
        public string RepositoryUrl { get; set; }
        public string DemoUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public enum ProjectCategory
    {
        ComputerVision,
        NaturalLanguageProcessing,
        MachineLearning,
        Robotics,
        VoiceAssistant,
        ReinforcementLearning,
        GenerativeAI,
        DataScience,
        Other
    }

    public enum ProjectVisibility
    {
        Public,
        Private,
        Unlisted
    }
}
