using System.ComponentModel.DataAnnotations;

namespace Api.Models;

public struct AccessCredentials
{
    [Required(AllowEmptyStrings = false)]
    public string Username { get; set; }

    [Required(AllowEmptyStrings = false)]
    public string Password { get; set; }
}
