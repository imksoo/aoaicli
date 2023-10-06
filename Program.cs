using System.CommandLine;
using System.CommandLine.Parsing;
using Azure;
using Azure.AI.OpenAI;

class Program
{

  static string AOAI_API_URI = Environment.GetEnvironmentVariable("AOAI_API_URI") ?? "";
  static string AOAI_API_KEY = Environment.GetEnvironmentVariable("AOAI_API_KEY") ?? "";
  static string AOAI_MODEL_NAME = Environment.GetEnvironmentVariable("AOAI_MODEL_NAME") ?? "gpt-35-turbo";

  static float AOAI_TEMPERATURE = float.Parse(Environment.GetEnvironmentVariable("AOAI_TEMPERATURE") ?? "1.0");
  static int AOAI_MAX_TOKENS = int.Parse(Environment.GetEnvironmentVariable("AOAI_MAX_TOKENS") ?? "800");
  static float AOAI_NUCLEUS_SAMPLING_FACTOR = float.Parse(Environment.GetEnvironmentVariable("AOAI_NUCLEUS_SAMPLING_FACTOR") ?? "0.95");
  static int AOAI_FREQUENCY_PENALTY = int.Parse(Environment.GetEnvironmentVariable("AOAI_FREQUENCY_PENALTY") ?? "0");
  static int AOAI_PRESENCE_PENALTY = int.Parse(Environment.GetEnvironmentVariable("AOAI_PRESENCE_PENALTY") ?? "0");

  static async Task Main(string[] args)
  {
    var rootCommand = new RootCommand();

    var apiUriOption = new Option<string>(name: "--api-uri", description: "The URI for an Azure OpenAI resource as retrieved from, for example, Azure Portal. This should include protocol and hostname. An example could be: https://my-resource.openai.azure.com", getDefaultValue: () => AOAI_API_URI);
    var apiKeyOption = new Option<string>(name: "--api-key", description: "A key credential used to authenticate to an Azure OpenAI resource.", getDefaultValue: () => AOAI_API_KEY);
    var modelNameOption = new Option<string>(name: "--model-name", description: "Deployment or Model name", getDefaultValue: () => AOAI_MODEL_NAME);
    rootCommand.AddGlobalOption(apiUriOption);
    rootCommand.AddGlobalOption(apiKeyOption);
    rootCommand.AddGlobalOption(modelNameOption);

    var temperatureOption = new Option<float>(name: "--temperature", description: "Gets or sets the sampling temperature to use that controls the apparent creativity of generated completions. Has a valid range of 0.0 to 2.0 and defaults to 1.0 if not otherwise specified.", getDefaultValue: () => AOAI_TEMPERATURE);
    var maxTokensOption = new Option<int>(name: "--max-tokens", description: "Gets the maximum number of tokens to generate. Has minimum of 0.", getDefaultValue: () => AOAI_MAX_TOKENS);
    var nucleusSamplingFactorOption = new Option<float>(name: "--nucleus-sampling-factor", description: "Gets or set a an alternative value to Temperature, called nucleus sampling, that causes the model to consider the results of the tokens with NucleusSamplingFactor probability mass.", getDefaultValue: () => AOAI_NUCLEUS_SAMPLING_FACTOR);
    // var frequencyPenaltyOption = new Option<int>(name: "--frequency-penalty", description: "Gets or sets a value that influences the probability of generated tokens appearing based on their cumulative frequency in generated text. Has a valid range of -2.0 to 2.0.", getDefaultValue: () => AOAI_FREQUENCY_PENALTY);
    // var presencePenaltyOption = new Option<int>(name: "--presence-penalty", description: "Gets or sets a value that influences the probability of generated tokens appearing based on their existing presence in generated text. Has a valid range of -2.0 to 2.0.", getDefaultValue: () => AOAI_PRESENCE_PENALTY);
    rootCommand.AddGlobalOption(temperatureOption);
    rootCommand.AddGlobalOption(maxTokensOption);
    rootCommand.AddGlobalOption(nucleusSamplingFactorOption);

    var messageArgument = new Argument<string[]>(name: "messages", description: "Chat messages");
    rootCommand.AddArgument(messageArgument);

    var chatMessages = new List<ChatMessage>();
    rootCommand.SetHandler((apiUri, apiKey, modelName, temperature, maxTokens, nucleusSamplingFactor, messages) =>
    {
      AOAI_API_URI = apiUri;
      AOAI_API_KEY = apiKey;
      AOAI_MODEL_NAME = modelName;

      AOAI_TEMPERATURE = temperature;
      AOAI_MAX_TOKENS = maxTokens;
      AOAI_NUCLEUS_SAMPLING_FACTOR = nucleusSamplingFactor;

      var roleMode = ChatRole.User;
      foreach (var msg in messages)
      {
        if (msg == "--system")
        {
          roleMode = ChatRole.System;
        }
        else if (msg == "--user")
        {
          roleMode = ChatRole.User;
        }
        else if (msg == "--assistant")
        {
          roleMode = ChatRole.Assistant;
        }
        else
        {
          chatMessages.Add(new ChatMessage(roleMode, msg));
        }
      }

      if (string.IsNullOrWhiteSpace(AOAI_API_URI))
      {
        Console.Error.WriteLine("Value cannot be null. AOAI_API_URI or --api-uri is required.");
        return;
      }
      if (string.IsNullOrWhiteSpace(AOAI_API_KEY))
      {
        Console.Error.WriteLine("Value cannot be null. AOAI_API_KEY or --api-key is required.");
        return;
      }

      var response = Chat(chatMessages);
      Console.WriteLine(response);
    }, apiUriOption, apiKeyOption, modelNameOption, temperatureOption, maxTokensOption, nucleusSamplingFactorOption, messageArgument);

    await rootCommand.InvokeAsync(args);
  }

  static string Chat(List<ChatMessage> chatMessages)
  {
    OpenAIClient client = new OpenAIClient(
            new Uri(AOAI_API_URI),
            new AzureKeyCredential(AOAI_API_KEY));

    var responseWithoutStream = client.GetChatCompletions(
      AOAI_MODEL_NAME,
      new ChatCompletionsOptions(chatMessages)
      {
        Temperature = AOAI_TEMPERATURE,
        MaxTokens = AOAI_MAX_TOKENS,
        NucleusSamplingFactor = AOAI_NUCLEUS_SAMPLING_FACTOR,
        FrequencyPenalty = AOAI_FREQUENCY_PENALTY,
        PresencePenalty = AOAI_PRESENCE_PENALTY
      });

    ChatCompletions completions = responseWithoutStream.Value;
    return string.Join("", completions.Choices.Select(c => c.Message.Content));
  }
}