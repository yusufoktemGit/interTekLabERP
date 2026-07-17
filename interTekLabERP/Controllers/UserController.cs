using interTekLabERP.Business.Services;
using interTekLabERP.Entities;
using interTekLabERP.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize(Roles = "Admin")]
public class UserController : Controller
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    public IActionResult Index()
    {
        var users = _userService.GetAll();

        return View(users);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(UserCreateVM model)
    {
        if (_userService.GetAll().Any(x => x.UserName == model.UserName))
        {
            ModelState.AddModelError(
                "UserName",
                "Bu kullanıcı adı zaten kayıtlı.");

            return View(model);
        }

        if (!ModelState.IsValid)
            return View(model);

        var user = new User
        {
            UserName = model.UserName,
            FullName = model.FullName,
            Role = model.Role,
            IsActive = model.IsActive,
            CreatedDate = DateTime.Now,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password)
        };

        _userService.Add(user);

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        var user = _userService.GetById(id);

        if (user == null)
            return NotFound();

        var vm = new UserEditVM
        {
            Id = user.Id,
            UserName = user.UserName,
            FullName = user.FullName,
            Role = user.Role,
            IsActive = user.IsActive
        };

        return View(vm);
    }

    [HttpPost]
    public IActionResult Edit(UserEditVM model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var user = _userService.GetById(model.Id);

        if (user == null)
            return NotFound();

        user.UserName = model.UserName;
        user.FullName = model.FullName;
        user.Role = model.Role;
        user.IsActive = model.IsActive;

        _userService.Update(user);

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public IActionResult ChangeStatus(int id)
    {
        _userService.ChangeStatus(id);

        return RedirectToAction(nameof(Index));
    }

}