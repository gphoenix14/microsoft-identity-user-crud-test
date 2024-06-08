using System.ComponentModel.DataAnnotations;

public class EditUserViewModel
{
    public required string Id { get; set; }

    [Required]
    [Display(Name = "Username")]
    public required string UserName { get; set; }

    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public required string Email { get; set; }

    [Display(Name = "First Name")]
    public required string FirstName { get; set; }

    [Display(Name = "Last Name")]
    public required string LastName { get; set; }
}
