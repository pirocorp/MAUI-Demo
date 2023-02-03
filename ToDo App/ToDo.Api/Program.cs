using Microsoft.EntityFrameworkCore;

using ToDo.Api.Data;

using ToDoModel = ToDo.Api.Models.ToDo;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ToDoDbContext>(
    opt => opt.UseSqlite(
        builder.Configuration.GetConnectionString("SqliteConnection")));

var app = builder.Build();

// app.UseHttpsRedirection();

app.MapGet("api/todo", async (ToDoDbContext context) =>
{
    var items = await context.ToDos.ToListAsync();
    return Results.Ok(items);
});

app.MapPost("api/todo", async (ToDoDbContext context, ToDoModel toDo) =>
{
    await context.AddAsync(toDo);
    await context.SaveChangesAsync();

    return Results.Created($"api/todo/{toDo.Id}", toDo);
});

app.MapPut("api/todo/{id:int}", async (ToDoDbContext context, int id, ToDoModel toDo) =>
{
    var todoModel = await context.ToDos.FindAsync(id);

    if (todoModel == null)
    {
        return Results.NotFound();
    }

    todoModel.ToDoName = toDo.ToDoName;
    await context.SaveChangesAsync();

    return Results.NoContent();
});

app.MapDelete("api/todo/{id:int}", async (ToDoDbContext context, int id) =>
{
    var todoModel = await context.ToDos.FindAsync(id);

    if (todoModel == null)
    {
        return Results.NotFound();
    }

    context.ToDos.Remove(todoModel);
    await context.SaveChangesAsync();

    return Results.NoContent();
});

app.Run();
