using System.Text;
using EC_locator.Core.Interfaces;
using EC_locator.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph;
using Microsoft.Identity.Web.Resource;
using System.Text.Json;
using System.Text.Json.Serialization;
using EC_locator.Core.Models;
using Microsoft.AspNetCore.Cors;

namespace API.Controllers;

[ApiController]
[Route("/[controller]")]
[EnableCors]
public class UsersController : ControllerBase
{
    private readonly ITeamsRepository _teamsRepository;

    public UsersController(ITeamsRepository teamsRepository)
    {
        _teamsRepository = teamsRepository;
    }
    
    [HttpGet]
    public async Task<string> GetUsers()
    {
        var employees = new List<Employee>();
        var users = await _teamsRepository.GetUsersAsync();
        foreach (var user in users)
        {
            employees.Add(new Employee(user.DisplayName, user.Id) );
        }
        
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        
        return JsonSerializer.Serialize(employees, options);
    }
}