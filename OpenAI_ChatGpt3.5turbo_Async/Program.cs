
using OpenAI.GPT3;
using OpenAI.GPT3.Managers;
using OpenAI.GPT3.ObjectModels;
using OpenAI.GPT3.ObjectModels.RequestModels;

Console.WriteLine("Hello, World!");

string? key = Environment.GetEnvironmentVariable("OPENAI_KEY");
string? question = args[0];
if (String.IsNullOrEmpty(key))
{
    Console.WriteLine("キーが環境変数に無い");
    return;
}

if (String.IsNullOrEmpty(question))
{
    Console.WriteLine("質問内容が無い");
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
        ChatMessage.FromUser("リンゴの英語はなんですか？"),
        ChatMessage.FromAssistance("Apple."),
        ChatMessage.FromUser(question)
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