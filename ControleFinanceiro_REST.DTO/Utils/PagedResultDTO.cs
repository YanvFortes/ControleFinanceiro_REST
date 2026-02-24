namespace ControleFinanceiro_REST.DTO.Utils;

public record PagedResultDTO<T>(IReadOnlyList<T> Items, int Total, string Search);
