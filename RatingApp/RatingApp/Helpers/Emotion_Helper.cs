using Microsoft.ProjectOxford.Emotion;
using Microsoft.ProjectOxford.Emotion.Contract;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RatingApp.Helpers
{
    static class Emotion_Helper
    {
        public const string AzureKey = "0c0858a6595a4e2e960152dbbe893cac";//<Give your azure key here>

        private static async Task<Emotion[]> GetHappiness(Stream stream)
        {
            var emotionClient = new EmotionServiceClient(AzureKey);
            var emotionResults = await emotionClient.RecognizeAsync(stream);
            
            if (emotionResults == null || emotionResults.Count() == 0)
            {
                throw new Exception("Can't detect face");
            }

            return emotionResults;
        }

        //Get the Emotion here
        public static async Task<float> GetAverageHappinessScore(Stream stream)
        {
            var emotionResults = await GetHappiness(stream);

            float happiness = 0.0f;

            foreach (var emotionResult in emotionResults)
            {
                happiness = happiness + emotionResult.Scores.Happiness; //emotion score here
            }

            return happiness / emotionResults.Count(); //Average incase multiple faces
        }

        public static int GetHappinessMessage(float score)
        {
            int iHappinessScore = 1; // Bare Minimum rating is 1.
            double result = Math.Round(score * 100, 0);
            
            if (result <= 20)
            {
                iHappinessScore = 1;
            }
            else if (result > 20 && result <= 40)
            {
                iHappinessScore = 2;
            }
            else if (result > 40 && result <= 60)
            {
                iHappinessScore = 3;
            }
            else if (result > 60 && result <= 80)
            {
                iHappinessScore = 4;
            }
            else if (result > 80 && result <= 100)
            {
                iHappinessScore = 5;
            }
            return iHappinessScore;
        }
    }
}