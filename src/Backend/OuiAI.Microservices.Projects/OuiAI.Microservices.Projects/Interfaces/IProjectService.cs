using OuiAI.Microservices.Projects.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OuiAI.Microservices.Projects.Interfaces
{
    public interface IProjectService
    {
        Task<ProjectDto> GetProjectByIdAsync(Guid id, Guid? currentUserId = null);
        Task<IEnumerable<ProjectSummaryDto>> GetProjectsAsync(ProjectFilter filter, int page = 1, int pageSize = 10, Guid? currentUserId = null);
        Task<IEnumerable<ProjectSummaryDto>> GetUserProjectsAsync(Guid userId, int page = 1, int pageSize = 10, Guid? currentUserId = null);
        Task<IEnumerable<ProjectSummaryDto>> GetFeaturedProjectsAsync(int count = 5, Guid? currentUserId = null);
        Task<IEnumerable<ProjectSummaryDto>> GetTrendingProjectsAsync(int count = 5, Guid? currentUserId = null);
        Task<IEnumerable<ProjectSummaryDto>> GetRecentProjectsAsync(int count = 5, Guid? currentUserId = null);
        Task<IEnumerable<ProjectSummaryDto>> GetRelatedProjectsAsync(Guid projectId, int count = 5, Guid? currentUserId = null);
        Task<ProjectDto> CreateProjectAsync(CreateProjectDto projectDto, Guid userId);
        Task<ProjectDto> UpdateProjectAsync(Guid id, UpdateProjectDto projectDto, Guid userId);
        Task DeleteProjectAsync(Guid id, Guid userId);
        Task<bool> LikeProjectAsync(Guid projectId, Guid userId, string userName);
        Task<bool> UnlikeProjectAsync(Guid projectId, Guid userId);
        Task<bool> RecordProjectViewAsync(Guid projectId, Guid userId);
        Task<int> GetProjectLikesCountAsync(Guid projectId);
        Task<int> GetProjectViewsCountAsync(Guid projectId);
        Task<bool> IsProjectLikedByUserAsync(Guid projectId, Guid userId);
    }

    public class ProjectFilter
    {
        public string SearchTerm { get; set; }
        public Guid? CategoryId { get; set; }
        public List<Guid> TagIds { get; set; }
        public ProjectSortBy SortBy { get; set; } = ProjectSortBy.Latest;
        public bool IncludePrivate { get; set; } = false;
    }

    public enum ProjectSortBy
    {
        Latest,
        Popular,
        MostLiked,
        MostViewed
    }
}
