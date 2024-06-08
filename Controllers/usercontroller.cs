using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

[Authorize]
public class UsersController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public UsersController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    // GET: Users
    [Authorize(Roles = "Admin,User")]
    public async Task<IActionResult> Index()
    {
        var users = await _userManager.Users.ToListAsync();
        return View(users);
    }

    // GET: Users/Create
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create()
    {
        var roles = await _roleManager.Roles.ToListAsync();
        var model = new UserViewModel
        {
            RoleList = roles.Select(r => new SelectListItem
            {
                Value = r.Name,
                Text = r.Name
            }).ToList()
        };
        return View(model);
    }

    // POST: Users/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(UserViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = new ApplicationUser
            {
                UserName = model.UserName,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                if (model.SelectedRoles != null && model.SelectedRoles.Any())
                {
                    await _userManager.AddToRolesAsync(user, model.SelectedRoles);
                }
                else
                {
                    await _userManager.AddToRoleAsync(user, "User");
                }
                return RedirectToAction(nameof(Index));
            }
            AddErrors(result);
        }

        var roles = await _roleManager.Roles.ToListAsync();
        model.RoleList = roles.Select(r => new SelectListItem
        {
            Value = r.Name,
            Text = r.Name
        }).ToList();

        return View(model);
    }

    // GET: Users/Edit/5
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(string id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        var roles = await _roleManager.Roles.ToListAsync();
        var userRoles = await _userManager.GetRolesAsync(user);
        var model = new UserViewModel
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            SelectedRoles = userRoles.ToList(),
            RoleList = roles.Select(r => new SelectListItem
            {
                Value = r.Name,
                Text = r.Name
            }).ToList()
        };
        return View(model);
    }

    // POST: Users/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(string id, UserViewModel model)
    {
        if (id != model.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
            {
                return NotFound();
            }

            user.UserName = model.UserName;
            user.Email = model.Email;
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                var rolesToAdd = model.SelectedRoles.Except(userRoles).ToList();
                var rolesToRemove = userRoles.Except(model.SelectedRoles).ToList();

                await _userManager.AddToRolesAsync(user, rolesToAdd);
                await _userManager.RemoveFromRolesAsync(user, rolesToRemove);

                return RedirectToAction(nameof(Index));
            }
            AddErrors(result);
        }

        var roles = await _roleManager.Roles.ToListAsync();
        model.RoleList = roles.Select(r => new SelectListItem
        {
            Value = r.Name,
            Text = r.Name
        }).ToList();

        return View(model);
    }

    // GET: Users/Delete/5
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(string id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        var result = await _userManager.DeleteAsync(user);
        if (result.Succeeded)
        {
            return RedirectToAction(nameof(Index));
        }

        AddErrors(result);
        return RedirectToAction(nameof(Index));
    }

    private void AddErrors(IdentityResult result)
    {
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }
    }
}
