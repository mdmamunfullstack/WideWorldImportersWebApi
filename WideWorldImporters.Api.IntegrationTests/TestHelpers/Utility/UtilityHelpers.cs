using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;

namespace WideWorldImporters.Api.IntegrationTests.TestHelpers.Utility
{
    internal static class UtilityHelpers
    {
        private static readonly Random _random = new Random();

        /// <summary>
        ///     Create a random alphanumeric string
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string RandomString(int length)
        {
            const string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(Chars, length)
                                        .Select(s => s[UtilityHelpers._random.Next(s.Length)]).ToArray());
        }

        /// <summary>
        ///     Write a message to the output window
        /// </summary>
        /// <param name="message"></param>
        /// <param name="optionalTitle">optional title</param>
        public static void WriteDebugString(string message, string optionalTitle = "Debug Log:") { Debug.WriteLine($"{optionalTitle} {DateTime.Now} - {message}"); }

        /// <summary>
        ///     Write response contents to the output window
        /// </summary>
        /// <param name="response"></param>
        public static void WriteResponseContent(HttpResponseMessage response)
        {
            using (var content = response.Content)
            {
                string contentString = content.ReadAsStringAsync().Result;
                WriteDebugString(contentString, "Debug Log - Response Content:");
            }
        }
    }
}