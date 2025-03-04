using OuiAI.Microservices.Projects.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OuiAI.Microservices.Projects.Interfaces
{
    public interface ITagService
    {
        Task<IEnumerable<TagDto>> GetAllTagsAsync();
        Task<TagDto> GetTagByIdAsync(Guid id);
        Task<IEnumerable<TagDto>> GetPopularTagsAsync(int count = 10);
        Task<IEnumerable<TagDto>> GetTagsByProjectIdAsync(Guid projectId);
        Task<TagDto> CreateTagAsync(CreateTagDto tagDto);
        Task<TagDto> UpdateTagAsync(Guid id, UpdateTagDto tagDto);
        Task DeleteTagAsync(Guid id);
        Task<IEnumerable<TagDto>> SearchTagsAsync(string searchTerm);
    }
}
