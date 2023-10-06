# README for the aoaicli program

## Description

`aoaicli` is a command-line tool that facilitates interactions with Azure OpenAI services. This program lets users send chat messages and get responses from models like GPT-4. The configuration is flexible with various options, including the ability to set the model, API endpoint, sampling temperature, and more.

## Usage

1. Clone the repository:

```sh
git clone https://github.com/imksoo/aoaicli.git
```

2. Navigate to the cloned directory:
```sh
cd aoaicli
```

3. Run the program:
```sh
dotnet run [options] [<messages>...]
```

## Arguments

- `<messages>`: Series of chat messages.

## Options

- `--api-uri <api-uri>`: Specifies the URI for an Azure OpenAI resource. This should include both protocol and hostname. For instance: `https://my-resource.openai.azure.com`.

- `--api-key <api-key>`: A key credential employed for authentication with an Azure OpenAI resource.

- `--model-name <model-name>`: Determines the deployment or model name. By default, it's set to `gpt-35-turbo`.

- `--temperature <temperature>`: This option sets the sampling temperature that impacts the apparent creativity of the generated completions. It has a valid range from 0.0 to 2.0. If not specified, the default value is 1.

- `--max-tokens <max-tokens>`: Indicates the maximum number of tokens to generate with a minimum allowable value of 0. It defaults to 800 if not specified.

- `--nucleus-sampling-factor <nucleus-sampling-factor>`: An alternative to the Temperature setting. Nucleus sampling makes the model consider token results with the NucleusSamplingFactor probability mass. The default value is 0.95 if not otherwise specified.

- `--version`: Displays the version details of the program.

- `-?, -h, --help`: Show help and usage information.

## Examples

To interact with a specific OpenAI endpoint with a given key and model:

```sh
dotnet run --api-uri https://my-resource.openai.azure.com --api-key YOUR_API_KEY --model-name gpt-35-turbo "Hello, how are you?"
```

To get a more creative response:

```sh
dotnet run --temperature 1.5 "Tell me a story about space."
```

## Using Response Files

You can also use response files (with an `.rsp` extension) to supply arguments to `aoaicli`. This can be especially helpful if you have a set of standard or frequently used arguments.

For instance, if you have a file named `example.rsp` with the following content:

```
--api-uri=https://my-resource.openai.azure.com
--api-key=YOUR_API_KEY
--model-name=gpt-35-turbo
--system
"You are an excellent programmer. Please respond faithfully to user inquiries."
--user
"Please teach me a simple way to use System.CommandLine in C#."
```

You can invoke `aoaicli` using:

```sh
dotnet run @example.rsp
```

This command will read the arguments specified in `example.rsp` and execute `aoaicli` accordingly.

## Notes

Make sure to replace placeholder values such as `YOUR_API_KEY` with your actual details. The program also supports reading from environment variables which can be a secure way to manage your credentials.

Always keep your API keys confidential to avoid misuse.

## License
This project is licensed under the terms of the MIT License. Please see the LICENSE file for details.