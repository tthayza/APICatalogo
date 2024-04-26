using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APICatalogo.Migrations
{
   
    public partial class PopulaProdutos : Migration
    {
       
        protected override void Up(MigrationBuilder mb)
        {
            mb.Sql("Insert into Produtos(Nome,Descricao,Preco,ImagemUrl,Estoque,DataCadastro,CategoriaId)" +
                "Values('Coca-Cola', 'Refrigerante de Cola 350ml', 5.45,'cocacola.jpg', 50,now(),1)");

            mb.Sql("Insert into Produtos(Nome,Descricao,Preco,ImagemUrl,Estoque,DataCadastro,CategoriaId)" +
                "Values('Lanche de frango', 'Frango com bacon', 8.45,'frango.jpg', 30,now(),2)");

            mb.Sql("Insert into Produtos(Nome,Descricao,Preco,ImagemUrl,Estoque,DataCadastro,CategoriaId)" +
                "Values('Brigadeiro', 'Brigadeiro belga', 3.45,'brigadeiro.jpg', 70,now(),3)");
        }

        
        protected override void Down(MigrationBuilder mb)
        {
            mb.Sql("Delete from Produtos");
        }
    }
}
