using System.ComponentModel.DataAnnotations;

namespace ControleFinanceiro_REST.DTO.Entities;

public class RoleDTO
{
    public string Id { get; set; } = string.Empty;

    [Required, StringLength(256)]
    public string Name { get; set; } = string.Empty;
}
