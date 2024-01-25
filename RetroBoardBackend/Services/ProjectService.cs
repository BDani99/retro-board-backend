using AutoMapper;
using RetroBoardBackend.Constans;
using RetroBoardBackend.Dtos;
using RetroBoardBackend.Dtos.Responses;
using RetroBoardBackend.Dtos.Result;
using RetroBoardBackend.Enums;
using RetroBoardBackend.Models;
using RetroBoardBackend.Repositories.Interfaces;
using RetroBoardBackend.Services.Interfaces;

namespace RetroBoardBackend.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository _projectRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IUserRepository _userRespoitory;
        private readonly IRetrospectiveRepository _retrospectiveRepository;
        private readonly IEntryRepository _entryRepository;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;

        public ProjectService(IProjectRepository projectRepository,
            IMapper mapper,
            ICategoryRepository categoryRepository,
            IUserRepository userRepository,
            IUserService userService,
            IRetrospectiveRepository retrospectiveRepository,
            IEntryRepository entryRepository)
        {
            _projectRepository = projectRepository;
            _mapper = mapper;
            _categoryRepository = categoryRepository;
            _userRespoitory = userRepository;
            _userService = userService;
            _retrospectiveRepository = retrospectiveRepository;
            _entryRepository = entryRepository;
        }

        public async Task<List<RetrospectiveResponseWithStats>> GetAllRetrospectivesByIdAsync(int id)
        {
            var retrospectives = await _projectRepository.GetAllRetrospectivesByIdAsync(id);

            var categories = await _categoryRepository.GetAllAsync();

            var retrospectivesWithStats = retrospectives.Where(x => x.StatsNeeded).ToList();
            var retrospectivesWithoutStats = retrospectives.Where(x => !x.StatsNeeded).ToList();

            var responses = new List<RetrospectiveResponseWithStats>();

            foreach (var retrospectiveWithStats in retrospectivesWithStats)
            {
                var retro = _mapper.Map<RetrospectiveResponseWithStats>(retrospectiveWithStats);
                var entries = retrospectiveWithStats.Entries;

                if (entries.Count() == 0)
                {
                    retro.LineChartStats = null;
                    retro.PieChartStats = null;
                }
                else
                {
                    retro.LineChartStats = CalculateLineChartStats(entries.ToList());
                    retro.PieChartStats = CalculatePieChartStats(entries.ToList(), categories);
                }

                var todoAmount = retrospectiveWithStats.Entries
                .Where(x => x.ColumnType.Equals(ColumnTypes.Todo)).Count();

                var wentWellAmount = retrospectiveWithStats.Entries
                    .Where(x => x.ColumnType.Equals(ColumnTypes.WentWell)).Count();

                var needsFixAmount = retrospectiveWithStats.Entries
                    .Where(x => x.ColumnType.Equals(ColumnTypes.NeedsFix)).Count();

                retro.EntryAmount = new EntryAmount
                {
                    TodoColumn = todoAmount,
                    WentWellColumn = wentWellAmount,
                    NeedsFixColumn = needsFixAmount,
                    Total = retrospectiveWithStats.Entries.Count()
                };

                responses.Add(retro);
            }

            foreach (var retrospectiveWithoutStats in retrospectivesWithoutStats)
            {
                var retro = _mapper.Map<RetrospectiveResponseWithStats>(retrospectiveWithoutStats);

                retro.LineChartStats = null;
                retro.PieChartStats = null;

                var todoAmount = retrospectiveWithoutStats.Entries
                .Where(x => x.ColumnType.Equals(ColumnTypes.Todo)).Count();

                var wentWellAmount = retrospectiveWithoutStats.Entries
                    .Where(x => x.ColumnType.Equals(ColumnTypes.WentWell)).Count();

                var needsFixAmount = retrospectiveWithoutStats.Entries
                    .Where(x => x.ColumnType.Equals(ColumnTypes.NeedsFix)).Count();

                retro.EntryAmount = new EntryAmount
                {
                    TodoColumn = todoAmount,
                    WentWellColumn = wentWellAmount,
                    NeedsFixColumn = needsFixAmount,
                    Total = retrospectiveWithoutStats.Entries.Count()
                };

                responses.Add(retro);
            }

            return responses;
        }

        public async Task<ProjectResult> CreateAsync(ProjectDto projectDto)
        {
            var project = _mapper.Map<Project>(projectDto);

            var categories = new List<Category>();

            foreach (var categoryId in projectDto.CategoryIds)
            {
                var category = await _categoryRepository.GetByIdAsync(categoryId);
                if (category == null)
                {
                    return new ProjectResult { ErrorMessage = ErrorMessages.CATEGORY_NOT_FOUND_WITH_THIS_ID };
                }

                categories.Add(category);
            }

            var users = new List<User>();
            foreach (var userId in projectDto.UserIds)
            {
                var actUser = await _userRespoitory.GetByIdAsync(userId);
                if (actUser == null)
                {
                    return new ProjectResult { ErrorMessage = ErrorMessages.USER_NOT_FOUND_WITH_THIS_ID };
                }
                users.Add(actUser);
            }

            project.Categories = categories;

            var user = await _userRespoitory.GetByIdAsync(project.PmId);
            if (user == null)
            {
                return new ProjectResult { ErrorMessage = ErrorMessages.USER_NOT_FOUND_WITH_THIS_ID };
            }

            project.AuthorUser = user;
            project.Users = users;
            await _projectRepository.CreateAsync(project);

            var postProjectResponse = _mapper.Map<PostProjectResponse>(project);
            foreach (var userResponse in postProjectResponse.Users)
            {
                var role = await _userRespoitory.GetUserRoleByIdAsync(userResponse.Id);
                if (role == null)
                {
                    return new ProjectResult { ErrorMessage = ErrorMessages.THERE_IS_NO_ROLE_FOR_THIS_USER };
                }

                userResponse.Role = role.Role.Name;
            }

            return new ProjectResult { PostProjectResponse = postProjectResponse };
        }

        public async Task<ProjectResult> UpdateAsync(int id, UpdateProjectDto updateProjectDto)
        {
            var project = await _projectRepository.GetByIdAsync(id);
            if (project == null)
            {
                return new ProjectResult { ErrorMessage = ErrorMessages.PROJECT_NOT_FOUND_WITH_THIS_ID };
            }

            var me = await _userService.GetMyselfAsync();
            if (me.Id != project.PmId)
            {
                return new ProjectResult { ErrorMessage = ErrorMessages.DONT_HAVE_PERMISSION_TO_MODIFY_THE_PROJECT };
            }

            _mapper.Map(updateProjectDto, project);
            if (updateProjectDto.UserIds.Count != 0)
            {
                var users = new List<User>();
                foreach (var userId in updateProjectDto.UserIds)
                {
                    var user = await _userRespoitory.GetByIdAsync(userId);
                    if (user == null)
                    {
                        return new ProjectResult { ErrorMessage = ErrorMessages.USER_NOT_FOUND_WITH_THIS_ID };
                    }

                    users.Add(user);
                }
                project.Users = users;
            }

            if (updateProjectDto.CategoryIds.Count != 0)
            {
                var categories = new List<Category>();
                foreach (var categoryId in updateProjectDto.CategoryIds)
                {
                    var category = await _categoryRepository.GetByIdAsync(categoryId);
                    if (category == null)
                    {
                        return new ProjectResult { ErrorMessage = ErrorMessages.CATEGORY_NOT_FOUND_WITH_THIS_ID };
                    }

                    categories.Add(category);
                }
                project.Categories = categories;
            }
            await _projectRepository.SaveChangesAsync();
            var projectResponse = _mapper.Map<ProjectResponse>(project);

            return new ProjectResult { ProjectResponse = projectResponse };
        }

        public async Task<ProjectResult> DeleteAsync(int id)
        {
            var project = await _projectRepository.GetByIdAsync(id);
            if (project is null)
            {
                return new ProjectResult { ErrorMessage = ErrorMessages.PROJECT_NOT_FOUND_WITH_THIS_ID };
            }

            var me = await _userService.GetMyselfAsync();
            if (me.Id != project.PmId)
            {
                return new ProjectResult { ErrorMessage = ErrorMessages.DONT_HAVE_PERMISSION_TO_MODIFY_THE_PROJECT };
            }

            project.IsDeleted = true;
            await _projectRepository.SaveChangesAsync();
            return new ProjectResult { };
        }

        public async Task<ProjectResult> GetProjectByIdAsync(int id)
        {
            var project = await _projectRepository.GetByIdAsync(id);
            if (project == null || project.IsDeleted == true)
            {
                return new ProjectResult { ErrorMessage = ErrorMessages.PROJECT_NOT_FOUND_WITH_THIS_ID };
            }

            var projectResponse = _mapper.Map<ProjectResponse>(project);

            var role = await _userRespoitory.GetUserRoleByIdAsync(projectResponse.PmUser.Id);
            if (role == null)
            {
                return new ProjectResult { ErrorMessage = ErrorMessages.THERE_IS_NO_ROLE_FOR_THIS_USER };
            }

            projectResponse.PmUser.Role = role.Role.Name;

            return new ProjectResult { ProjectResponse = projectResponse };
        }

        public async Task<ProjectResult> GetAllProjectAsync()
        {
            var projects = await _projectRepository.GetAllAsync();

            var projectResponses = projects.Select(x => _mapper.Map<ProjectResponse>(x));

            foreach (var projectResponse in projectResponses)
            {
                var role = await _userRespoitory.GetUserRoleByIdAsync(projectResponse.PmUser.Id);
                if (role == null)
                {
                    return new ProjectResult { ErrorMessage = ErrorMessages.THERE_IS_NO_ROLE_FOR_THIS_USER };
                }

                projectResponse.PmUser.Role = role.Role.Name;
            }

            return new ProjectResult { ProjectResponses = projectResponses };
        }

        private LineChartStats CalculateLineChartStats(List<Entry> entries)
        {
            var wentWellEntryAmount = entries
                        .Where(x => x.ColumnType == Enums.ColumnTypes.WentWell).Count();

            var needsFixEntryAmount = entries
                .Where(x => x.ColumnType == Enums.ColumnTypes.NeedsFix).Count();

            double wentWellRatio = 0;
            double needsFixRatio = 0;

            if (wentWellEntryAmount == 0)
            {
                wentWellRatio = 0;
            }
            else
            {
                wentWellRatio = (double)wentWellEntryAmount /
                (wentWellEntryAmount + needsFixEntryAmount);
            }

            if (needsFixEntryAmount == 0)
            {
                needsFixRatio = 0;
            }
            else
            {
                needsFixRatio = (double)needsFixEntryAmount /
                    (wentWellEntryAmount + needsFixEntryAmount);
            }

            var lineChartStats = new LineChartStats
            {
                WentWellRatio = Math.Round(wentWellRatio, 2),
                NeedsFixRatio = Math.Round(needsFixRatio, 2)
            };

            return lineChartStats;
        }

        private PieChartStats CalculatePieChartStats(List<Entry> entries, List<Category> categories)
        {
            //PieChart statisztikák - Jól ment oszlop
            var wentWellColumnStats = new List<CategoryWithRatio>();

            var wentWellColumn = entries
                .Where(x => x.ColumnType == Enums.ColumnTypes.WentWell);

            var categoryStats = new Dictionary<string, int>();
            for (int i = 0; i < categories.Count; i++)
            {
                var categoryName = categories[i].Name;
                var categoryCount = 0;
                categoryStats.TryAdd(categoryName, categoryCount);
            }

            foreach (var entry in wentWellColumn)
            {
                var categoriesOfEntry = entry.Categories;
                foreach (var category in categoriesOfEntry)
                {
                    var categoryName = category.Name;
                    categoryStats[categoryName]++;
                }
            }

            foreach (KeyValuePair<string, int> kvp in categoryStats)
            {
                var categoryName = kvp.Key;
                var categoryCount = kvp.Value;

                if (categoryCount > 0)
                {
                    var category = entries.SelectMany(entry => entry.Categories)
                        .FirstOrDefault(category => category.Name.Equals(categoryName));

                    var categoryWithRatio = _mapper.Map<CategoryWithRatio>(category);

                    var sumOfCategories = categoryStats.Values.Sum();
                    double ratio = (double)categoryCount / sumOfCategories;
                    categoryWithRatio.Ratio = Math.Round(ratio, 4);

                    wentWellColumnStats.Add(categoryWithRatio);
                }
            }

            //PieChart statisztikák - Jól ment oszlop

            var needsFixColumnStats = new List<CategoryWithRatio>();

            var needsFixColumn = entries
                .Where(x => x.ColumnType == Enums.ColumnTypes.NeedsFix);

            foreach (var key in categoryStats.Keys.ToList())
            {
                categoryStats[key] = 0;
            }

            foreach (var entry in needsFixColumn)
            {
                var categoriesOfEntry = entry.Categories;
                foreach (var category in categoriesOfEntry)
                {
                    var categoryName = category.Name;
                    categoryStats[categoryName]++;
                }
            }

            foreach (KeyValuePair<string, int> kvp in categoryStats)
            {
                var categoryName = kvp.Key;
                var categoryCount = kvp.Value;

                if (categoryCount > 0)
                {
                    var category = entries.SelectMany(entry => entry.Categories)
                        .FirstOrDefault(category => category.Name.Equals(categoryName));

                    var categoryWithRatio = _mapper.Map<CategoryWithRatio>(category);

                    var sumOfCategories = categoryStats.Values.Sum();
                    double ratio = (double)categoryCount / sumOfCategories;
                    categoryWithRatio.Ratio = Math.Round(ratio, 4);

                    needsFixColumnStats.Add(categoryWithRatio);
                }
            }

            var pieChartStats = new PieChartStats
            {
                WentWellColumnStats = wentWellColumnStats,
                NeedsFixColumnStats = needsFixColumnStats
            };

            return pieChartStats;
        }

    }
}
