using Microsoft.EntityFrameworkCore;
using RetroBoardBackend.Data;
using RetroBoardBackend.Models;
using RetroBoardBackend.Repositories.Interfaces;

namespace RetroBoardBackend.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly ApplicationDbContext _context;

        public ProjectRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task<List<Project>> GetAllAsync()
        {
            return _context.Projects.Where(x => x.IsDeleted == false).Include(x => x.Categories)
                .Include(x => x.AuthorUser).ToListAsync();
        }

        public async Task<ICollection<Retrospective>> GetAllRetrospectivesByIdAsync(int id)
        {
            var projects = await _context.Projects
                .Include(x => x.Retrospectives)
                .ThenInclude(x => x.Entries)
                .ThenInclude(x => x.Categories)
                .FirstOrDefaultAsync(x => x.Id.Equals(id));

            if (projects == null)
            {
                return new List<Retrospective>();
            }

            return projects.Retrospectives;

        }

        public async Task<Project> CreateAsync(Project project)
        {
            _context.Projects.Add(project);
            await _context.SaveChangesAsync();
            return project;
        }

        public Task SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }

        public Task<Project?> GetByIdAsync(int id)
        {
            return _context.Projects.Include(x => x.AuthorUser).
                Include(x => x.Categories).FirstOrDefaultAsync(x => x.Id == id);
        }

        public Task<List<Project>> GetProjectsWhereAuthorWithUserIdAsync(int userId)
        {
            return _context.Projects
                .Include(p => p.Categories)
                .Include(p => p.Users)
                .Where(x => x.PmId == userId && x.IsDeleted == false)
                .ToListAsync();
        }
    }
}
