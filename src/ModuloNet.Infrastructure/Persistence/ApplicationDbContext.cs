using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ModuloNet.Application.Abstractions;
using ModuloNet.Application.Features.Courses;
using ModuloNet.Infrastructure.Identity;
using ModuloNet.Infrastructure.Persistence.Configurations;

namespace ModuloNet.Infrastructure.Persistence;

public sealed class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Course> Courses => Set<Course>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public void AddCourse(Course course) => Courses.Add(course);

    public async Task<bool> DeleteCourseAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var course = await Courses.FindAsync([id], cancellationToken);
        if (course is null) return false;
        Courses.Remove(course);
        return true;
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(CourseConfiguration).Assembly);
    }
}
