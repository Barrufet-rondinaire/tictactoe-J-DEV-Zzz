using System.Net.Http.Json;
using System.Text.RegularExpressions;

namespace TicTacToe;

class Program
{
    static async Task Main()
    {
        using var client = new HttpClient();

        var comentaris = await client.GetFromJsonAsync<List<string>>(
            "http://localhost:8080/jugadors"
        );

        Dictionary<string, string> jugadors = new Dictionary<string, string>();
        Dictionary<string, int> victories = new Dictionary<string, int>();

        string patternPais = @"representa\w*\s\w*\s(?<country>[\w-]*)";
        string patternPersona = @"participant\w*\s(?<Persona>\w*\s\w*)";

        if (comentaris != null)
        {
            foreach (var comentari in comentaris)
            {
                Match matchPersona = Regex.Match(comentari, patternPersona);
                Match matchPais = Regex.Match(comentari, patternPais);
                
                if (matchPersona.Success && matchPais.Success)
                {
                    string persona = matchPersona.Groups["Persona"].Value;
                    string pais = matchPais.Groups["country"].Value;

                    if (!jugadors.ContainsKey(persona))
                    {
                        jugadors.Add(persona, pais);
                        victories[persona] = 0;
                    }
                }
            }
        }

        for (int i = 1; i <= 10000; i++)
        {
            var partida = await client.GetStringAsync($"http://localhost:8080/partida/{i}");
            string[] files = partida.Split('\n', StringSplitOptions.RemoveEmptyEntries);

            if (files.Length != 3 || files.Any(f => f.Length != 3)) continue;

            char[,] tauler = new char[3, 3];

            for (int j = 0; j < 3; j++)
            {
                for (int k = 0; k < 3; k++)
                {
                    tauler[j, k] = files[j][k];
                }
            }

            string? guanyador = DeterminarGuanyador(tauler);

            if (guanyador != null && victories.ContainsKey(guanyador))
            {
                victories[guanyador]++;
            }
        }

        Console.WriteLine("Participants en la competició:");
        foreach (var jugador in jugadors)
        {
            Console.WriteLine($"Pais: {jugador.Value}, Nom: {jugador.Key}");
        }

        var millorJugador = victories.OrderByDescending(v => v.Value).First();
        string paisGuanyador = jugadors[millorJugador.Key];

        Console.WriteLine($"\nEl guanyador del campionat és: {millorJugador.Key} ({paisGuanyador}) amb {millorJugador.Value} victòries.");
    }

    static string? DeterminarGuanyador(char[,] tauler)
    {
        for (int i = 0; i < 3; i++)
        {
            if (tauler[i, 0] == tauler[i, 1] && tauler[i, 1] == tauler[i, 2] && tauler[i, 0] != '.')
                return tauler[i, 0] == '0' ? "Jugador 1" : "Jugador 2";

            if (tauler[0, i] == tauler[1, i] && tauler[1, i] == tauler[2, i] && tauler[0, i] != '.')
                return tauler[0, i] == '0' ? "Jugador 1" : "Jugador 2";
        }

        if (tauler[0, 0] == tauler[1, 1] && tauler[1, 1] == tauler[2, 2] && tauler[0, 0] != '.')
            return tauler[0, 0] == '0' ? "Jugador 1" : "Jugador 2";

        if (tauler[0, 2] == tauler[1, 1] && tauler[1, 1] == tauler[2, 0] && tauler[0, 2] != '.')
            return tauler[0, 2] == '0' ? "Jugador 1" : "Jugador 2";

        return null;
    }
}
