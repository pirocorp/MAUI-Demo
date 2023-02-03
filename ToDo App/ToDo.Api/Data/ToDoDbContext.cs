namespace ToDo.Api.Data;

using Microsoft.EntityFrameworkCore;

using ToDo.Api.Models;

public class ToDoDbContext : DbContext
{
    public ToDoDbContext(DbContextOptions<ToDoDbContext> options)
        : base(options)
    { }

    public DbSet<ToDo> ToDos => this.Set<ToDo>();
}
