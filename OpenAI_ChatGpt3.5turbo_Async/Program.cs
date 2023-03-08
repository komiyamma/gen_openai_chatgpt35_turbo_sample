
using OpenAI.GPT3;
using OpenAI.GPT3.Managers;
using OpenAI.GPT3.ObjectModels;
using OpenAI.GPT3.ObjectModels.RequestModels;

Console.WriteLine("Hello, World!");

string? key = Environment.GetEnvironmentVariable("OPENAI_KEY");
if (String.IsNullOrEmpty(key))
{
    Console.WriteLine("キーが環境変数に無い");
    return;
}

var openAiService = new OpenAIService(new OpenAiOptions()
{
    ApiKey = key
});

var completionResult = openAiService.ChatCompletion.CreateCompletionAsStream(new ChatCompletionCreateRequest
{
    Messages = new List<ChatMessage>
    {
        ChatMessage.FromSystem("You are a helpful assistant."),
        // ChatMessage.FromUser("pythonの効率的な学び方はなんですか？"),
        // ChatMessage.FromAssistance("The Los Angeles Dodgers won the World Series in 2020."),
        ChatMessage.FromUser("C#の効率的な学び方はなんですか？")
    },
    Model = Models.ChatGpt3_5Turbo,
    MaxTokens = 1000 //optional
});
await foreach (var completion in completionResult)
{
    if (completion.Successful)
    {
        Console.Write(completion.Choices.FirstOrDefault()?.Message.Content);
    }
    else
    {
        if (completion.Error == null)
        {
            throw new Exception("Unknown Error");
        }

        Console.WriteLine($"{completion.Error.Code}: {completion.Error.Message}");
    }
}
Console.WriteLine("Complete");