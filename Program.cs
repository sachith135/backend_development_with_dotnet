using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebApplication1.Middleware;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var users = new List<User>
{
    new User { Name = "Alice", Age = 25 },
    new User { Name = "Bob", Age = 30 }
};

// Configure middleware pipeline
app.UseErrorHandler();
app.UseTokenAuthentication();
app.UseRequestResponseLogging();

// Read all users
app.MapGet("/users", () => users);

// Read user by name
app.MapGet("/users/{name}", (string name) =>
{
    var user = users.FirstOrDefault(u => u.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    return user != null ? Results.Ok(user) : Results.NotFound();
});

// Create a new user
app.MapPost("/users", (User newUser) =>
{
    users.Add(newUser);
    return Results.Created($"/users/{newUser.Name}", newUser);
});

// Update an existing user
app.MapPut("/users/{name}", (string name, User updatedUser) =>
{
    var user = users.FirstOrDefault(u => u.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    if (user == null)
    {
        return Results.NotFound();
    }

    user.Name = updatedUser.Name;
    user.Age = updatedUser.Age;

    return Results.Ok(user);
});

// Delete a user
app.MapDelete("/users/{name}", (string name) =>
{
    var user = users.FirstOrDefault(u => u.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    if (user == null)
    {
        return Results.NotFound();
    }

    users.Remove(user);
    return Results.NoContent();
});

app.Run();

public class User
{
    public string Name { get; set; }
    public int Age { get; set; }
}
