using interTekLabERP.Entities;

namespace interTekLabERP.Business.Services;

public interface IUserService
{
    User? Login(string userName, string password);
    List<User> GetAll();

    User? GetById(int id);

    void Add(User user);

    void Update(User user);

    void Delete(int id);
    void ChangeStatus(int id);
}