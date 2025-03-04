using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OuiAI.Common.DTOs;
using OuiAI.Common.Models;

namespace OuiAI.Common.Interfaces
{
    public interface IProjectService
    {
        Task<Project> GetProjectByIdAsync(Guid projectId);
        Task<PaginatedResult<Project>> GetProjectsAsync(int pageNumber, int pageSize, ProjectCategory? category = null, string searchQuery = null);
        Task<PaginatedResult<Project>> GetUserProjectsAsync(Guid userId, int pageNumber, int pageSize);
        Task<Project> CreateProjectAsync(Project project);
        Task<Project> UpdateProjectAsync(Project project);
        Task<bool> DeleteProjectAsync(Guid projectId);
        Task<bool> LikeProjectAsync(Guid projectId, Guid userId);
        Task<bool> UnlikeProjectAsync(Guid projectId, Guid userId);
        Task<PaginatedResult<Project>> GetFeaturedProjectsAsync(int pageNumber, int pageSize);
        Task<PaginatedResult<Project>> GetTrendingProjectsAsync(int pageNumber, int pageSize);
    }
}
