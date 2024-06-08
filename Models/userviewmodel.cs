using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class UserViewModel
{
    public string? Id { get; set; }

    [Required]
    [Display(Name = "Username")]
    public string UserName { get; set; }

    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; }

    [Display(Name = "First Name")]
    public string? FirstName { get; set; }

    [Display(Name = "Last Name")]
    public string? LastName { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string? Password { get; set; }

    [Display(Name = "Roles")]
    public List<string>? SelectedRoles { get; set; }

    public List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>? RoleList { get; set; }
}
