namespace ToDo.Maui.DataServices;

using ToDo.Maui.Models;

public interface IToDoDataService
{
    Task<List<ToDo>> GetAllToDos();

    Task AddToDo(ToDo toDo);

    Task UpdateToDo(ToDo todo);

    Task DeleteToDo(int id);
}
