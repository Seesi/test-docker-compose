using System;
using Microsoft.EntityFrameworkCore;

namespace docker_compose_demo_api;

public class AppDbContext : DbContext
{
    public DbSet<Blog> Blogs { get; set; }
    public DbSet<Post> Posts { get; set; }

 public AppDbContext(DbContextOptions options)
    : base(options)
    {
    }
}
