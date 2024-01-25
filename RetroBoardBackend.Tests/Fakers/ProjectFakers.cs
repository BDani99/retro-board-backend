using Bogus;
using RetroBoardBackend.Dtos;
using RetroBoardBackend.Dtos.Responses;
using RetroBoardBackend.Dtos.Result;
using RetroBoardBackend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetroBoardBackend.Tests.Fakers
{
    internal class ProjectFakers
    {
        public static Faker<Project> CreateProjectFaker()
        {
            return new Faker<Project>()
                .RuleFor(x => x.Id, y => y.IndexFaker + 1)
                .RuleFor(x => x.PmId, y => y.IndexFaker + 1)
                .RuleFor(x => x.Name, y => y.Internet.UserName())
                .RuleFor(x => x.Description, y => y.Random.String())
                .RuleFor(x => x.HasRetroboard, y => y.Random.Bool())
                .RuleFor(x => x.IsDeleted, false);
        }

        public static Faker<UpdateProjectDto> CreateUpdateProjectDtoFaker()
        {
            var categoryIds = new List<int>() { 1, 2, 3 };
            var userIds = new List<int>() { 1, 2, 3 };
            return new Faker<UpdateProjectDto>()
                .RuleFor(x => x.Image, y => y.Random.Bool() ? y.Internet.Avatar() : null)
                .RuleFor(x => x.Description, y => y.Random.Bool() ? y.Random.String() : null)
                .RuleFor(x => x.HasRetroboard, y => y.Random.Bool())
                .RuleFor(x => x.CategoryIds, categoryIds)
                .RuleFor(x => x.UserIds, userIds);
        }

        public static Faker<ProjectResponse> CreateProjectResponseFaker(Project project)
        {
            var userFaker = UserFakers.CreateUserFakerWithSpecificId(project.PmId);
            var user = userFaker.Generate();

            var userResponseFaker = UserFakers.CreateUserResponseFaker(user);
            var userResponse = userResponseFaker.Generate();

            return new Faker<ProjectResponse>()
                .RuleFor(x => x.Id, project.Id)
                .RuleFor(x => x.PmUser, userResponse)
                .RuleFor(x => x.Name, project.Name)
                .RuleFor(x => x.Description, project.Description)
                .RuleFor(x => x.HasRetroboard, project.HasRetroboard);
        }

        public static Faker<MyProjectResponse> CreateMyProjectResponseFaker(Project project, User user)
        {
            var userResponseFaker = UserFakers.CreateUserResponseFaker(user);
            var userResponse = userResponseFaker.Generate();

            return new Faker<MyProjectResponse>()
                .RuleFor(x => x.Id, project.Id)
                .RuleFor(x => x.AuthorUser, userResponse)
                .RuleFor(x => x.Name, project.Name)
                .RuleFor(x => x.Description, project.Description)
                .RuleFor(x => x.UserCount, 0);
        }

        public static Faker<Project> CreateProjectWithSpecificAuthorFaker(int userId, int projectsCount)
        {
            return new Faker<Project>()
                .RuleFor(x => x.Id, y => y.IndexFaker + 1 + projectsCount)
                .RuleFor(x => x.PmId, userId)
                .RuleFor(x => x.Name, y => y.Internet.UserName())
                .RuleFor(x => x.Description, y => y.Random.String())
                .RuleFor(x => x.HasRetroboard, y => y.Random.Bool())
                .RuleFor(x => x.IsDeleted, false);
        }

        public static Faker<ProjectResult> CreateProjectResult(
            IEnumerable<ProjectResponse>? projectResponses = null,
            PostProjectResponse? postProjectResponse = null,
            ProjectResponse? projectResponse = null,
            string? errorMessage = null
            )
        {
            return new Faker<ProjectResult>()
                .RuleFor(x => x.ProjectResponses, projectResponses)
                .RuleFor(x => x.PostProjectResponse, postProjectResponse)
                .RuleFor(x => x.ProjectResponse, projectResponse)
                .RuleFor(x => x.ErrorMessage, errorMessage);
        }

        public static Faker<ProjectDto> CreateProjectDto(IEnumerable<Category> categories, IEnumerable<User> users)
        {
            var categoryIds = new List<int>();
            foreach (var category in categories)
            {
                categoryIds.Add(category.Id);
            }

            var userIds = new List<int>();
            foreach (var user in users)
            {
                userIds.Add(user.Id);
            }

            return new Faker<ProjectDto>()
                .RuleFor(x => x.Name, y => y.Random.Word())
                .RuleFor(x => x.Description, y => y.Random.String())
                .RuleFor(x => x.CategoryIds, categoryIds)
                .RuleFor(x => x.UserIds, userIds);
        }


        public static Faker<Project> CreateProjectFromDto(ProjectDto projectDto)
        {
            var categoryFaker = CategoryFakers.CreateCategoryFaker();
            var categories = categoryFaker.Generate(5);
            var userFaker = UserFakers.CreateUserFaker();
            var users = userFaker.Generate(3);

            return new Faker<Project>()
                .RuleFor(x => x.Id, y => y.IndexFaker + 1)
                .RuleFor(x => x.PmId, y => y.IndexFaker + 1)
                .RuleFor(x => x.Name, projectDto.Name)
                .RuleFor(x => x.Description, projectDto.Description)
                .RuleFor(x => x.HasRetroboard, false)
                .RuleFor(x => x.IsDeleted, false)
                .RuleFor(x => x.Users, users)
                .RuleFor(x => x.Categories, categories);
        }

        public static Faker<PostProjectResponse> CreatePostProjectResponse(Project project)
        {
            var categoriesResponse = new List<CategoryResponse>();
            foreach (var category in project.Categories)
            {
                var categoryResponseFaker = CategoryFakers.CreateCategoryResponseFaker(category);
                var categoryResponse = categoryResponseFaker.Generate();
                categoriesResponse.Add(categoryResponse);
            }

            var usersResponse = new List<UserResponse>();
            foreach (var user in project.Users)
            {
                var userResponseFaker = UserFakers.CreateUserResponseFaker(user);
                var userResponse = userResponseFaker.Generate();
                usersResponse.Add(userResponse);
            }

            return new Faker<PostProjectResponse>()
                .RuleFor(x => x.Id, project.Id)
                .RuleFor(x => x.Name, project.Name)
                .RuleFor(x => x.Description, project.Description)
                .RuleFor(x => x.Categories, categoriesResponse)
                .RuleFor(x => x.Users, usersResponse);
        }

        public static Faker<Project> UpdateOriginalResult(UpdateProjectDto updateProjectDto, Project originalProject)
        {
            var categoryFaker = CategoryFakers.CreateCategoryFaker();
            var categories = categoryFaker.Generate(3);

            var userFaker = UserFakers.CreateUserFaker();
            var users = userFaker.Generate(3);

            return new Faker<Project>()
                .RuleFor(x => x.Id, originalProject.Id)
                .RuleFor(x => x.PmId, originalProject.PmId)
                .RuleFor(x => x.Image, y => updateProjectDto is null ? originalProject.Image : updateProjectDto.Image)
                .RuleFor(x => x.Name, originalProject.Name)
                .RuleFor(x => x.Description, y => updateProjectDto is null ? originalProject.Description : updateProjectDto.Description)
                .RuleFor(x => x.HasRetroboard, y => updateProjectDto is null ? originalProject.HasRetroboard : updateProjectDto.HasRetroboard)
                .RuleFor(x => x.IsDeleted, originalProject.IsDeleted)
                .RuleFor(x => x.AuthorUser, originalProject.AuthorUser)
                .RuleFor(x => x.Users, y => updateProjectDto is null ? originalProject.Users : users)
                .RuleFor(x => x.Categories, y => updateProjectDto is null ? originalProject.Categories : categories);
        }

    }
}
