using OuiAI.Microservices.Projects.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OuiAI.Microservices.Projects.Interfaces
{
    public interface IMediaService
    {
        Task<IEnumerable<ProjectMediaDto>> GetMediaByProjectIdAsync(Guid projectId);
        Task<ProjectMediaDto> GetMediaByIdAsync(Guid id);
        Task<ProjectMediaDto> CreateMediaAsync(CreateMediaDto mediaDto);
        Task<ProjectMediaDto> UpdateMediaAsync(Guid id, UpdateMediaDto mediaDto);
        Task DeleteMediaAsync(Guid id);
        Task ReorderMediaAsync(Guid projectId, List<Guid> mediaIdsInOrder);
    }
}
