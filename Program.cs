using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Lab4;

class Program
{
    private static readonly HttpClient client = new HttpClient();

    static async Task Main(string[] args)
    {
        client.DefaultRequestHeaders.UserAgent.Add(
            new ProductInfoHeaderValue("Lab4", "1.0"));

        Console.WriteLine("Hämtar data...");

        await GetDotNetRepositories();
        Console.WriteLine(new string('-', 50));
        await GetZipCodeInfo();

        Console.WriteLine("\nKlar. Tryck på valfri tangent för att avsluta");
        Console.ReadKey();
    }
    private static async Task GetDotNetRepositories()
    {
        var url = "https://api.github.com/orgs/dotnet/repos";
        
        try
        {
            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"GitHub-Anrop misslyckades. Statuskod: {response.StatusCode}");
                return;
            }

            var content = await response.Content.ReadAsStringAsync();

            var repositories = JsonSerializer.Deserialize<List<GitHubProjektRepo>>(content);

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Repositories under .NET Foundation:");
            Console.ResetColor();

            if (repositories == null || repositories.Count == 0)
            {
                Console.WriteLine("Inga repositories hittades.");
                return;
            }

            foreach (var repo in repositories)
            {
                Console.WriteLine($"\nName: {repo.Name}" +
                                  $"\nHomepage: {repo.Homepage}" +
                                  $"\nGitHub: {repo.HtmlUrl}" +
                                  $"\nDescription: {repo.Description}" +
                                  $"\nWatchers: {repo.Watchers}" +
                                  $"\nLast push: {repo.PushedAt}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ett fel inträffade vid hämtning från GitHub: {ex.Message}");
        }
    }

    private static async Task GetZipCodeInfo()
    {
        var url = "https://api.zippopotam.us/us/07645";

        try
        {
            var response = await client.GetAsync(url);

            if(!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Zippotam-anrop misslyckades. Statuskod: {response.StatusCode}");
                return;
            }

            var content = await response.Content.ReadAsStringAsync();
            
            var zipCodeInfo = JsonSerializer.Deserialize<ZipCodeInfo>(content);

            if (zipCodeInfo != null && zipCodeInfo.Places !=null && zipCodeInfo.Places.Count > 0)
            {
                var place = zipCodeInfo.Places[0];

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Here is the extra detail:");
                Console.ResetColor();

                Console.WriteLine($"ZIP Code {zipCodeInfo.PostCode}" +
                    $"\nPlace: {place.PlaceName}" +
                    $"\nState: {place.State}" +
                    $"\nLatitude: {place.Latitude}" +
                    $"\nLongitude: {place.Longitude}");
            }
            else
            {
                Console.WriteLine("No data found for the ZIP code.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ett fel inträffade vid hämtning från Zippotam: {ex.Message}");
        }
    }
}