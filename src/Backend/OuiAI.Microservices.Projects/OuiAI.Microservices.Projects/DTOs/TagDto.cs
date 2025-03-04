using System;

namespace OuiAI.Microservices.Projects.DTOs
{
    public class TagDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int ProjectCount { get; set; }
    }

    public class CreateTagDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class UpdateTagDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
