using BLL.Interfaces;
using Domain;
using Domain.Models;

namespace BLL.Services;

public class UserService : IUserService
{
    private readonly ApplicationContext _applicationContext;

    public UserService(ApplicationContext applicationContext)
    {
        _applicationContext = applicationContext;
    }
    
    public int SaveChanges()
    {
        return _applicationContext.SaveChanges();
    }

    public User GetUserByName(string username)
    {
        return _applicationContext.Users.FirstOrDefault(u => u.Username == username);
    }

    public void AddUser(User user)
    {
        _applicationContext.Users.Add(user);
    }
}