using AutoMapper;
using RetroBoardBackend.Dtos;
using RetroBoardBackend.Dtos.Responses;
using RetroBoardBackend.Enums;
using RetroBoardBackend.Models;
using RetroBoardBackend.Repositories.Interfaces;
using RetroBoardBackend.Services.Interfaces;

namespace RetroBoardBackend.Services
{
    public class EntryReactionService : IEntryReactionService
    {
        private readonly IEntryReactionReposiotry _entryReactionRepository;
        private readonly IEntryService _entryService;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public EntryReactionService(IEntryReactionReposiotry entryReactionRepository,
            IMapper mapper,
            IUserService userService,
            IEntryService entryService)
        {
            _entryReactionRepository = entryReactionRepository;
            _mapper = mapper;
            _userService = userService;
            _entryService = entryService;
        }

        public async Task<EntryReactionResponse> CreateAsync(EntryReactionDto entryReactionDto)
        {
            var reaction = _mapper.Map<EntryReaction>(entryReactionDto);

            //AfterMap: EntryReactionDto -> EntryReaction

            var me = await _userService.GetMyselfAsync();
            reaction.UserId = me.Id;

            switch (entryReactionDto.ReactionType)
            {
                case "Like":
                    reaction.ReactionTypes = ReactionTypes.Like;
                    break;

                case "Dislike":
                    reaction.ReactionTypes = ReactionTypes.Dislike;
                    break;
            }

            await _entryReactionRepository.CreateAsync(reaction);

            var response = _mapper.Map<EntryReactionResponse>(reaction);

            //AfterMap: EntryReaction -> EntryReactionResponse

            var reacterUser = await _userService.GetByIdAsync(reaction.UserId);
            response.User = reacterUser!;
            response.Entry = await _entryService.GetByIdAsync(reaction.EntryId);
            response.ReactionType = reaction.ReactionTypes.ToString();

            return response;
        }

        public async Task<bool> AlreadyReacted(int userId, int entryId)
        {
            var alreadyDeleted = await _entryReactionRepository.AlreadyReacted(userId, entryId);
            return alreadyDeleted;
        }

        public async Task<bool> DeleteAsync(int userId, int entryId)
        {
            return await _entryReactionRepository.DeleteAsync(userId, entryId);
        }
    }
}
