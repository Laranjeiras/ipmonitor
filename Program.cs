
using System.Net;
using System.Text;
using System.Text.Json;

static async Task<IPAddress?> GetExternalIpAddress()
{
    var externalIpString = (await new HttpClient().GetStringAsync("http://icanhazip.com"))
        .Replace("\\r\\n", "").Replace("\\n", "").Trim();
    if (!IPAddress.TryParse(externalIpString, out var ipAddress)) return null;
    return ipAddress;
}

static async Task SendToDiscord(string ip) {
    var content = $"{ip} - {Dns.GetHostName()}";
    var msg = new {
            content = content
        };

    using StringContent jsonContent = new(
        JsonSerializer.Serialize(msg),
        Encoding.UTF8,
        "application/json");


    var http = new HttpClient();

    await http.PostAsync(
        "https://discord.com/api/webhooks/1094642529615491183/lf2fs8eUizR6SJWdvZv0oUd80OZLgscXPnNFfBkVWr1EZb66pHFw3FrVppxwZ8jAK7bN",
        jsonContent
        );
}

var ip = await GetExternalIpAddress();
string  ipAnterior = string.Empty;

if (ip is null)
{
    throw new Exception("IP NÃO ENCONTRADO");
}

if (File.Exists("ip.txt")) {
    ipAnterior = await File.ReadAllTextAsync("ip.txt") ?? string.Empty;
}

if (ip.MapToIPv4().ToString() == ipAnterior) {
    System.Console.WriteLine("IP NÃO MODIFICADO");
    return;
}

System.Console.WriteLine(ip);
await SendToDiscord(ip.MapToIPv4().ToString());
await File.WriteAllTextAsync("ip.txt", ip.MapToIPv4().ToString());
