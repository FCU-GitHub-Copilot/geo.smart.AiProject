using System;

namespace Geo.Smart.AiAgentHub.EfGenerator
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            EfCoreGenerator.Generate();
            Console.WriteLine("done!");
        }
    }
}
