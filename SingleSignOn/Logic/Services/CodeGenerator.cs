using System;

namespace SingleSignOn.Logic.Services
{
    public class CodeGenerator
    {
        public static string Generate()
        {
            return new Random().Next(100000, 999999).ToString("000000");
        }
    }
}
