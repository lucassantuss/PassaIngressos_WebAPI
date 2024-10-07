namespace PassaIngressos_WebAPI.Dto
{
    public class ItemTabelaGeralDto
    {
        public int IdItemTabelaGeral { get; set; }

        public string Sigla { get; set; }

        public string Descricao { get; set; }

        public TabelaGeralDto TabelaGeral { get; set; }
    }
}