using Amazon.Lambda.Core;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace HelloWorld;

public class Program
{
    public static void Handle(Stream stream)
    {
        using var reader = new StreamReader(stream);
        Console.WriteLine("Hello, World!");
        Console.WriteLine(reader.ReadToEnd());
        Console.WriteLine("Bye");
    }
}