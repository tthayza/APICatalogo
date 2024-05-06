namespace APICatalogo.Pagination
{
    public class ProdutosFiltroPreco : QueryStringsParameters
    {
        public decimal? Preco {  get; set; }
        public string? PrecoCriterio { get; set; } //"maior", "menor" ou "igual"
    }
}
