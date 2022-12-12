using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Text.RegularExpressions;

var input = File.ReadAllLines("./chat.txt");

var messages = new List<Message>();

foreach (var line in input)
{
    var match = Regex.Match(line, @"\d{2}\/\d{2}\/\d{4}, \d{2}:\d{2} - \w* \w*:");

    if (!match.Success)
    {
        var anonSenderMatch = Regex.Match(line, @"\d{2}\/\d{2}\/\d{4}, \d{2}:\d{2} - [\w*\-., ]*");

        if (anonSenderMatch.Success)
        {
            continue;
        }

        // new line of the previous message
        messages.Last().Content += $"\n{line}";
    }
    else
    {
        messages.Add(new Message
        {
            Id = Guid.NewGuid(),
            SentDate = DateOnly.Parse(line.Split(",")[0]),
            SentTime = TimeOnly.Parse(line.Split(", ")[1].Split(" -")[0]),
            Sender = line.Split(" -")[1].Split(":")[0].Trim(),
            Content = line.Split(": ")[1]
        });
    }
}

File.WriteAllText("../../../output.json", JsonConvert.SerializeObject(
    messages,
    new JsonSerializerSettings 
    { 
        ContractResolver = new DefaultContractResolver 
        { 
            NamingStrategy = new CamelCaseNamingStrategy
            { 
                OverrideSpecifiedNames = false 
            } 
        },
        Formatting = Formatting.Indented,
    }));


class Message
{
    public Guid Id { get; set; }

    public DateOnly SentDate { get; set; }

    public TimeOnly SentTime { get; set; }

    public string Sender { get; set; }

    public string Content { get; set; }
}