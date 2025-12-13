using SEM.Services;

class Program
{
    public static void Main(string[] args)
    {
        var parserOne = new JsonToModelParser();
        var parserTwo = new ModelToCodeParser();
        var json =
            "{\n  \"array\": [\n    1,\n    2,\n    3\n  ],\n  \"boolean\": true,\n  \"color\": \"gold\",\n  \"null\": null,\n  \"number\": 123,\n  \"objct\": {\n    \"a\": \"b\",\n    \"c\": \"d\"\n  },\n  \"string\": \"Hello World\"\n}";
        Console.WriteLine(parserTwo.ParseModelToCode(parserOne.ParseJsonToModel("Class1", json)));
        
    }
}