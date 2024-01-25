using AutoMapper;
using RetroBoardBackend.Dtos;
using RetroBoardBackend.Models;
using RetroBoardBackend.Services.Interfaces;

namespace RetroBoardBackend.Mappers.Actions
{
    public class SetProjectAction : IMappingAction<ProjectDto, Project>
    {
        private readonly ITokenService _tokenService;

        public SetProjectAction(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        public void Process(ProjectDto source, Project destination,
          ResolutionContext context)
        {
            var pmId = _tokenService.GetMyId();
            destination.PmId = pmId;
        }
    }
}
