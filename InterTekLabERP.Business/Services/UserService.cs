using BCrypt.Net;
using interTekLabERP.Data;
using interTekLabERP.Entities;

namespace interTekLabERP.Business.Services;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;

    public UserService(ApplicationDbContext context)
    {
        _context = context;
    }

    public User? Login(string userName, string password)
    {
        var user = _context.Users.FirstOrDefault(x => x.UserName == userName && x.IsActive);

        if (user == null)
            return null;

        //string hash = BCrypt.Net.BCrypt.HashPassword("123456");

        bool result = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);

        return result ? user : null;
    }

    public List<User> GetAll()
    {
        return _context.Users
            .OrderBy(x => x.FullName)
            .ToList();
    }

    public User? GetById(int id)
    {
        return _context.Users
            .FirstOrDefault(x => x.Id == id);
    }

    public void Add(User user)
    {
        _context.Users.Add(user);
        _context.SaveChanges();
    }

    public void Update(User user)
    {
        _context.Users.Update(user);
        _context.SaveChanges();
    }

    public void Delete(int id)
    {
        var user = _context.Users.FirstOrDefault(x => x.Id == id);

        if (user == null)
            return;

        user.IsActive = false;

        _context.SaveChanges();
    }

    public void ChangeStatus(int id)
    {
        var user = _context.Users.FirstOrDefault(x => x.Id == id);

        if (user == null)
            return;

        user.IsActive = !user.IsActive;

        _context.SaveChanges();
    }
}