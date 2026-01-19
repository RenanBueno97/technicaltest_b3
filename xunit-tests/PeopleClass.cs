using System;
using System.Linq;

class PeopleClass
{
    static void Main()
    {
        var people = new[] {
            new { Name = "José", DateBirth = new DateTime(1982, 03, 27), Active = true },
            new { Name = "Leandro", DateBirth = new DateTime(1978,04,03), Active = false },
            new { Name = "Pedro", DateBirth = new DateTime(1980,05,24), Active = true }
        };

        // Obter nomes de pessoas nascidas a partir de 1980
        var nomes = people
            .Where(p => p.DateBirth.Year >= 1980)
            .Select(p => new { p.Name, p.DateBirth });

        // Exibir os resultados
        foreach (var pessoa in nomes)
        {

            Console.WriteLine(pessoa.Name + "-" + pessoa.DateBirth.ToString("dd/MM/yyyy"));
        }
    }
}
