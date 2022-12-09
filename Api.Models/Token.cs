using System.ComponentModel.DataAnnotations;

namespace Api.Models;

public struct Token
{
    [Required(AllowEmptyStrings = false)]
    public string Value { get; set; }
    
    [Required()]
    public DateTime ExpiresIn { get; set; }
}
