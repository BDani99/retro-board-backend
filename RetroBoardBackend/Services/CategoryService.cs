using AutoMapper;
using RetroBoardBackend.Dtos;
using RetroBoardBackend.Dtos.Responses;
using RetroBoardBackend.Models;
using RetroBoardBackend.Repositories.Interfaces;
using RetroBoardBackend.Services.Interfaces;

namespace RetroBoardBackend.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IMapper _mapper;

        public CategoryService(ICategoryRepository categoryRepository,
            IProjectRepository projectRepository,
            IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _projectRepository = projectRepository;
            _mapper = mapper;
        }

        public async Task<List<CategoryResponse>> GetAllAsync()
        {
            var categories = await _categoryRepository.GetAllAsync();
            return _mapper.Map<List<CategoryResponse>>(categories);
        }

        public async Task<PostProjectResponse> CreateAsync(ProjectDto projectDto)
        {
            var project = _mapper.Map<Project>(projectDto);
            await _projectRepository.CreateAsync(project);

            var proj = _mapper.Map<PostProjectResponse>(project);
            return proj;
        }

        public async Task<CategoryResponse> GetByIdAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            return _mapper.Map<CategoryResponse>(category);
        }
    }
}