
using OpenAI.GPT3;
using OpenAI.GPT3.Managers;
using OpenAI.GPT3.ObjectModels;
using OpenAI.GPT3.ObjectModels.RequestModels;
using OpenAI.GPT3.ObjectModels.ResponseModels;
using static OpenAI.GPT3.ObjectModels.Models;

class Program
{
    // OpenAIのキーの取得
    static string? GetOpenAIKey()
    {
        string? key = Environment.GetEnvironmentVariable("OPENAI_KEY");
        if (String.IsNullOrEmpty(key))
        {
            Console.WriteLine("キーが環境変数に無い");
        }
        return key;
    }

    // 質問内容の取得
    static string? GetQuestion()
    {
        string? question = "";

        Console.Write("-- 質問をどうぞ --\n");
        question = Console.ReadLine();

        if (String.IsNullOrEmpty(question))
        {
            Console.WriteLine("質問内容が無い\n");
        }
        Console.Write("\n");

        if (String.IsNullOrEmpty(question))
        {
            return null;
        }

        if (question.ToUpper() == "チャットを終了")
        {
            return null;
        }

        return question;
    }

    // OpenAIサービスのインスタンス。一応保持
    static OpenAIService? openAiService = null;

    static OpenAIService? ConnectOpenAIService(string key)
    {
        try
        {
            var openAiService = new OpenAIService(new OpenAiOptions()
            {
                ApiKey = key
            });

            return openAiService;
        }
        catch (Exception ex)
        {
            Console.WriteLine("OpenAIのサービスに接続できません。:\n" + ex);
        }
        return null;
    }

    // OpenAIサービスのインスタンスのクリア。多分disconnectみたいなメソッドはない。
    static void ClearOpenAIService()
    {
        openAiService = null;
    }

    // OpenAIサービスのインスタンスのクリア。多分disconnectみたいなメソッドはない。
    static List<ChatMessage> messageList = new();

    // チャットのエンジンやオプション。過去のチャット内容なども渡す。
    static IAsyncEnumerable<ChatCompletionCreateResponse> ReBuildPastChatContents()
    {
        var key = GetOpenAIKey();
        if (key == null)
        {
            throw new Exception("NullOpenAIKeyException");
        }

        openAiService = ConnectOpenAIService(key);
        if (openAiService == null)
        {
            throw new Exception("NullOpenAIServiceException)");
        }

        var list = new List<ChatMessage>();
        list.Add(ChatMessage.FromSystem("You are a helpful assistant."));

        foreach (var mes in messageList)
        {
            list.Add(mes);
        }

        var options = new ChatCompletionCreateRequest
        {
            Messages = list,
            Model = Models.ChatGpt3_5Turbo,
            MaxTokens = 1000
        };

        var completionResult = openAiService.ChatCompletion.CreateCompletionAsStream(options);
        return completionResult;
    }

    // チャットの反復
    static async Task RepeatChat()
    {
        while (true)
        {
            string? question = GetQuestion();
            if (question == null)
            {
                break;
            }
            messageList.Add(ChatMessage.FromUser(question));
            var completionResult = ReBuildPastChatContents();

            string answer_sum = "";
            await foreach (var completion in completionResult)
            {
                if (completion.Successful)
                {
                    string? str = completion.Choices.FirstOrDefault()?.Message.Content;
                    answer_sum += str ?? "";
                    Console.Write(str);
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

            messageList.Add(ChatMessage.FromAssistance(answer_sum));
            Console.WriteLine("\n\n-- 完了 --\n\n");
        }

    }

    // メイン
    public static async Task Main(String[] args)
    {
        await RepeatChat();
    }
}


