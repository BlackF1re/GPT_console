using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        var apiKey = "";
        var client = new HttpClient();
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
        var url = "https://api.openai.com/v1/chat/completions";
        Console.WriteLine("Введите сообщение (или нажмите Enter для завершения): ");
        while (true)
        {
            var userMessage = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(userMessage))
            {
                break;
            }

            var content = new StringContent(
                $@"{{
                    ""model"": ""gpt-3.5-turbo-1106"",
                    ""messages"": [
                        {{
                            ""role"": ""system"",
                            ""content"": ""You are an assistant.""
                        }},
                        {{
                            ""role"": ""user"",
                            ""content"": ""{userMessage}""
                        }}
                    ]
                }}",
                Encoding.UTF8,
                "application/json"
            );

            var cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(30)); // Установка времени ожидания ответа (30 секунд)

            var response = await client.PostAsync(url, content, cts.Token);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var responseObject = JsonSerializer.Deserialize<ChatResponse>(responseContent);
                Console.WriteLine("\n> " + responseObject.choices[0].message.content + "\n");
            }
            else
            {
                Console.WriteLine($"Ошибка: {response.ReasonPhrase}");
            }
        }
    }

    public class ChatResponse
    {
        public Choice[] choices { get; set; }
    }

    public class Choice
    {
        public Message message { get; set; }
    }

    public class Message
    {
        public string content { get; set; }
    }
}