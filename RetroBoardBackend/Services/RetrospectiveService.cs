using AutoMapper;
using RetroBoardBackend.Dtos;
using RetroBoardBackend.Dtos.Responses;
using RetroBoardBackend.Enums;
using RetroBoardBackend.Models;
using RetroBoardBackend.Repositories.Interfaces;
using RetroBoardBackend.Services.Interfaces;

namespace RetroBoardBackend.Services
{
    public class RetrospectiveService : IRetrospectiveService
    {
        private readonly IRetrospectiveRepository _retrospectiveRepository;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public RetrospectiveService(IRetrospectiveRepository retrospectiveRepository,
            IMapper mapper,
            IUserService userService)
        {
            _retrospectiveRepository = retrospectiveRepository;
            _mapper = mapper;
            _userService = userService;
        }

        public async Task<RetrospectiveResponse> GetByIdAsync(int id)
        {
            var retrospective = await _retrospectiveRepository.GetByIdAsync(id);
            var response = _mapper.Map<RetrospectiveResponse>(retrospective);

            if (retrospective != null)
            {
                var todoAmount = retrospective.Entries
                .Where(x => x.ColumnType.Equals(ColumnTypes.Todo)).Count();

                var wentWellAmount = retrospective.Entries
                    .Where(x => x.ColumnType.Equals(ColumnTypes.WentWell)).Count();

                var needsFixAmount = retrospective.Entries
                    .Where(x => x.ColumnType.Equals(ColumnTypes.NeedsFix)).Count();

                response.EntryAmount = new EntryAmount
                {
                    TodoColumn = todoAmount,
                    WentWellColumn = wentWellAmount,
                    NeedsFixColumn = needsFixAmount,
                    Total = retrospective.Entries.Count()
                };
            }

            return response;
        }

        public async Task<List<EntryResponse>> GetAllEntriesByIdAsync(int id)
        {
            var entries = await _retrospectiveRepository.GetAllEntriesByIdAsync(id);
            var me = await _userService.GetMyselfAsync();

            //AfterMap: List<Entry> -> List<EntryResponse>

            var responses = new List<EntryResponse>();

            foreach (var entry in entries)
            {
                var response = _mapper.Map<EntryResponse>(entry);

                response.Retrospective = await GetByIdAsync(entry.RetrospectiveId);

                var assignee = await _userService.GetByIdAsync(entry.AssigneeId);
                response.Assignee = assignee!;

                var author = await _userService.GetByIdAsync(entry.AuthorId);
                response.Author = author!;

                response.ColumnType = entry.ColumnType.ToString();

                response.Categories = _mapper.Map<List<CategoryResponse>>(entry.Categories);

                var likeAmount = entry.EntryReactions
                    .Where(x => x.ReactionTypes.Equals(ReactionTypes.Like)).Count();

                var dislikeAmount = entry.EntryReactions
                    .Where(x => x.ReactionTypes.Equals(ReactionTypes.Dislike)).Count();

                var currentUserReaction = entry.EntryReactions.Where(x => x.UserId.Equals(me!.Id)
                && x.EntryId.Equals(entry.Id)).FirstOrDefault();

                if (currentUserReaction != null)
                {
                    response.CurrentUserReaction = currentUserReaction.ReactionTypes.ToString();
                }

                response.ReactionAmount = new ReactionAmount
                {
                    LikeAmount = likeAmount,
                    DislikeAmount = dislikeAmount
                };

                responses.Add(response);
            }

            return responses;
        }

        public async Task<RetrospectiveResponse> CreateAsync(RetrospectiveDto retrospectiveDto)
        {
            var retrospective = _mapper.Map<Retrospective>(retrospectiveDto);
            await _retrospectiveRepository.CreateAsync(retrospective);

            return _mapper.Map<RetrospectiveResponse>(retrospective);
        }

        public async Task UpdateAsync(int id, UpdateRetrospectiveDto updateRetrospectiveDto)
        {
            var retrospective = await _retrospectiveRepository.GetByIdAsync(id);

            _mapper.Map(updateRetrospectiveDto, retrospective);
            await _retrospectiveRepository.SaveChangesAsync();
        }
    }
}
