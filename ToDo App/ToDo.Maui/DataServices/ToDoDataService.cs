namespace ToDo.Maui.DataServices;

using System.Diagnostics;
using System.Text;
using System.Text.Json;

using ToDo.Maui.Models;

public class ToDoDataService : IToDoDataService
{
    private readonly HttpClient httpClient;
    // https://learn.microsoft.com/en-us/dotnet/maui/data-cloud/local-web-services?wt.mc_id=lesjackson_dotnetmaui-mvp-video&view=net-maui-7.0
    private readonly string baseAddress;
    private readonly string url;
    private readonly string endpoint;
    private readonly JsonSerializerOptions jsonSerializerOptions;

    public ToDoDataService(HttpClient httpClient)
    {
        // this.httpClient = new HttpClient() -> this can lead to socket exhaustion
        this.httpClient = httpClient;

        this.baseAddress = DeviceInfo.Platform == DevicePlatform.Android
            ? "http://10.0.2.2:5254"
            : "https://localhost:7247";

        this.url = $"{this.baseAddress}/api";
        this.endpoint = $"{this.url}/todo";
        this.jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    public async Task<List<ToDo>> GetAllToDos()
    {
        var toDos = new List<ToDo>();

        if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
        {
            Debug.WriteLine("---> No internet access...");
            return toDos;
        }

        try
        {
            var response = await this.httpClient.GetAsync(this.endpoint);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                toDos = JsonSerializer.Deserialize<List<ToDo>>(content, this.jsonSerializerOptions);
            }
            else
            {
                Debug.WriteLine("---> Non Http 2xx response");
            }
        }
        catch (Exception e)
        {
            Debug.WriteLine($"Exception: {e.Message}");
        }

        return toDos;
    }

    public async Task AddToDo(ToDo toDo)
    {
        if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
        {
            Debug.WriteLine("---> No internet access...");
            return;
        }

        try
        {
            var jsonToDo = JsonSerializer.Serialize(toDo, this.jsonSerializerOptions);
            var content = new StringContent(jsonToDo, Encoding.UTF8, "application/json");

            var response = await this.httpClient.PostAsync(this.endpoint, content);

            Debug.WriteLine(
                response.IsSuccessStatusCode 
                ? "Successfully created ToDo" 
                : "---> Non Http 2xx response");
        }
        catch (Exception e)
        {
            Debug.WriteLine($"Exception: {e.Message}");
        }
    }

    public async Task UpdateToDo(ToDo todo)
    {
        if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
        {
            Debug.WriteLine("---> No internet access...");
            return;
        }

        try
        {
            var jsonToDo = JsonSerializer.Serialize(todo, this.jsonSerializerOptions);
            var content = new StringContent(jsonToDo, Encoding.UTF8, "application/json");

            var response = await this.httpClient.PutAsync($"{this.endpoint}/{todo.Id}", content);

            Debug.WriteLine(
                response.IsSuccessStatusCode 
                    ? "Successfully created ToDo" 
                    : "---> Non Http 2xx response");
        }
        catch (Exception e)
        {
            Debug.WriteLine($"Exception: {e.Message}");
        }
    }

    public async Task DeleteToDo(int id)
    {
        if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
        {
            Debug.WriteLine("---> No internet access...");
            return;
        }

        try
        {
            var response = await this.httpClient.DeleteAsync($"{this.endpoint}/{id}");

            Debug.WriteLine(
                response.IsSuccessStatusCode 
                    ? "Successfully created ToDo" 
                    : "---> Non Http 2xx response");
        }
        catch (Exception e)
        {
            Debug.WriteLine($"Exception: {e.Message}");
        }
    }
}
