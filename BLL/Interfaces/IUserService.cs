using Domain.Models;

namespace BLL.Interfaces;

public interface IUserService
{
    int SaveChanges();

    User GetUserByName(string username);

    void AddUser(User user);
}