// начальные данные
List<TaskSchema> tasks = new List<TaskSchema>
{
    new() { Id = Guid.NewGuid(), Title = "WebAPI", Description = "Create WebAPI", Status = TaskSchema.TaskStatus.Done, CreatedAt = DateTime.Now},
    new() { Id = Guid.NewGuid(), Title = "CRUD", Description = "Create CRUD methods", Status = TaskSchema.TaskStatus.ToDo, CreatedAt = DateTime.Now},
    new() { Id = Guid.NewGuid(), Title = "HTML", Description = "Create HTML for testing API", Status = TaskSchema.TaskStatus.Doing, CreatedAt = DateTime.Now},
};
 
var builder = WebApplication.CreateBuilder();
var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapGet("/api/tasks", (string? status) =>
{
    if (string.IsNullOrWhiteSpace(status))
        return Results.Json(tasks);

    if (!Enum.TryParse<TaskSchema.TaskStatus>(status, true, out var parsedStatus))
        return Results.BadRequest(new { message = "Некорректный статус" });

    var filteredTasks = tasks.Where(t => t.Status == parsedStatus).ToList();
    return Results.Json(filteredTasks);
});

app.MapGet("/api/tasks/{id}", (Guid id) =>
{
    // получаем пользователя по id
    TaskSchema? task = tasks.FirstOrDefault(t => t.Id == id);
    // если не найден, отправляем статусный код и сообщение об ошибке
    if (task == null)  return Results.NotFound(new { message = "Task не найден" });
 
    // если пользователь найден, отправляем его
    return Results.Json(task);
});
 
app.MapDelete("/api/tasks/{id}", (Guid id) =>
{
    // получаем пользователя по id
    TaskSchema? task = tasks.FirstOrDefault(t => t.Id == id);
 
    // если не найден, отправляем статусный код и сообщение об ошибке
    if (task == null) return Results.NotFound(new { message = "Task не найден" });
 
    // если пользователь найден, удаляем его
    tasks.Remove(task);
    return Results.Json(task);
});
 
app.MapPost("/api/tasks", (TaskSchema task)=>{
 
    // устанавливаем id для нового пользователя
    task.Id = Guid.NewGuid();
    // добавляем пользователя в список
    tasks.Add(task);
    return task;
});
 
app.MapPut("/api/tasks", (TaskSchema taskData) => {
 
    // получаем пользователя по id
    var task = tasks.FirstOrDefault(u => u.Id == taskData.Id);
    // если не найден, отправляем статусный код и сообщение об ошибке
    if (task == null) return Results.NotFound(new { message = "Пользователь не найден" });
    // если пользователь найден, изменяем его данные и отправляем обратно клиенту
     
    task.Title = taskData.Title;
    task.Status = taskData.Status;
    return Results.Json(task);
});
 
app.Run();
 
public class TaskSchema
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";

    public enum TaskStatus
    {
        ToDo,
    Doing,
    Done
}
    public TaskStatus Status {get;set;} = TaskStatus.ToDo;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}