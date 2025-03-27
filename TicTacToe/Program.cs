using System.Net.Http.Json;
namespace TicTacToe;

class Program
{
    static async Task Main()
    {
       using var client = new HttpClient();
       {
           var r = await client.GetFromJsonAsync <List<string>> (
               "http://localhost:8080/jugadors"
           );

           foreach (var comentari in r)
           {
               
           }
           {
               
           }
       }
    }
}
