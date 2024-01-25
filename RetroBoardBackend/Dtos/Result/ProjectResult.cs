using RetroBoardBackend.Dtos.Responses;

namespace RetroBoardBackend.Dtos.Result
{
    public class ProjectResult
    {
        public ProjectResponse? ProjectResponse { get; set; }
        public IEnumerable<ProjectResponse>? ProjectResponses { get; set; }

        public PostProjectResponse? PostProjectResponse { get; set; }
        public string? ErrorMessage { get; set; }

        public override bool Equals(object? obj)
        {
            return ((obj is ProjectResult projectResult) &&
                ((projectResult.ProjectResponse is not null && projectResult.ProjectResponse.Equals(ProjectResponse)) ||
                (projectResult.ProjectResponses is not null && projectResult.ProjectResponses.Equals(ProjectResponses)) ||
                (projectResult.PostProjectResponse is not null && projectResult.PostProjectResponse.Equals(PostProjectResponse)) ||
                (projectResult.ErrorMessage is not null && projectResult.ErrorMessage.Equals(ErrorMessage))));
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(ProjectResponse, ProjectResponses, PostProjectResponse, ErrorMessage);
        }
    }
}
