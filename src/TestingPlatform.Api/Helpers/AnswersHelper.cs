using System;

namespace TestingPlatform.Api.Helpers
{
    public static class AnswersHelper
    {
        public static bool CheckAnswer(string userAnswer, string rightAnswer)
        {
            return string.Equals(rightAnswer?.Trim(), userAnswer?.Trim(), StringComparison.InvariantCultureIgnoreCase);
        }
    }
}