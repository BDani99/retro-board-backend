using AutoMapper;
using RetroBoardBackend.Dtos;
using RetroBoardBackend.Dtos.Responses;
using RetroBoardBackend.Enums;
using RetroBoardBackend.Models;
using RetroBoardBackend.Repositories.Interfaces;
using RetroBoardBackend.Services.Interfaces;

namespace RetroBoardBackend.Services
{
    public class EntryService : IEntryService
    {
        private readonly IEntryRepository _entryRepository;
        private readonly IRetrospectiveService _retrospectiveService;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public EntryService(IEntryRepository entryRepository,
            IMapper mapper,
            ICategoryRepository categoryRepository,
            IRetrospectiveService retrospectiveService,
            IUserService userService)
        {
            _entryRepository = entryRepository;
            _mapper = mapper;
            _categoryRepository = categoryRepository;
            _retrospectiveService = retrospectiveService;
            _userService = userService;
        }

        public async Task<EntryResponse?> GetByIdAsync(int id)
        {
            var entry = await _entryRepository.GetByIdAsync(id);

            if (entry == null)
            {
                return null;
            }

            var response = _mapper.Map<EntryResponse>(entry);

            var author = await _userService.GetByIdAsync(entry!.AuthorId);
            if (author != null)
            {
                response.Author = author;
            }

            var assignee = await _userService.GetByIdAsync(entry!.AssigneeId);
            if (assignee != null)
            {
                response.Assignee = assignee;
            }

            response.Retrospective =
                _mapper.Map<RetrospectiveResponse>(entry.Retrospective);

            return response;
        }

        public async Task<List<CategoryResponse>> GetAllCategoriesByIdAsync(int id)
        {
            var categories = await _entryRepository.GetAllCategoriesByIdAsync(id);
            return _mapper.Map<List<CategoryResponse>>(categories);
        }

        public async Task<EntryResponse> CreateAsync(EntryDto entryDto)
        {
            var entry = _mapper.Map<Entry>(entryDto);

            //AfterMap: EntryDto -> Entry
            foreach (var categoryId in entryDto.CategoryIds)
            {
                var category = await _categoryRepository.GetByIdAsync(categoryId);
                if (category != null)
                {
                    entry.Categories.Add(category);
                }
            }

            var me = await _userService.GetMyselfAsync();
            entry.AuthorId = me.Id;

            switch (entryDto.ColumnType)
            {
                case "WentWell":
                    entry.ColumnType = ColumnTypes.WentWell;
                    break;
                case "NeedsFix":
                    entry.ColumnType = ColumnTypes.NeedsFix;
                    break;
                case "Todo":
                    entry.ColumnType = ColumnTypes.Todo;
                    break;
            }

            await _entryRepository.CreateAsync(entry);

            var response = _mapper.Map<EntryResponse>(entry);

            //AfterMap: Entry -> EntryResponse

            response.Retrospective =
                await _retrospectiveService.GetByIdAsync(entry.RetrospectiveId);

            var assignee = await _userService.GetByIdAsync(entry.AssigneeId);
            response.Assignee = assignee!;

            var author = await _userService.GetByIdAsync(entry.AuthorId);
            response.Author = author!;

            response.ColumnType = entry.ColumnType.ToString();

            return response;
        }
    }
}
