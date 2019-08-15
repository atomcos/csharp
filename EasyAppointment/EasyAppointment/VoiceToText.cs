using Microsoft.CognitiveServices.Speech;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace EasyAppointment
{
    class VoiceToText
    {
        private static string s1 = "033d657853e94bb4b752f88357d170d2";
        private static string s2 = "canadacentral";

        private TaskCompletionSource<int> stopBaseRecognitionTaskCompletionSource;

        private static void UpdateTb(TextBox tb, string text, CheckBox ckb, bool isChecked)
        {
            tb.Text = text;
            ckb.IsChecked = isChecked;
        }

        public static async void VoiceToTextOnce(TextBox tb, CheckBox ckb)
        {
            var config = SpeechConfig.FromSubscription(s1, s2);
            string rlt = "";
            using (var recognizer = new SpeechRecognizer(config))
            {
                var result = await recognizer.RecognizeOnceAsync();
                if (result.Reason == ResultReason.RecognizedSpeech)
                {
                    rlt = Regex.Replace(result.Text, @"[^ A-Za-z0-9]+", ""); 
                }
                else if (result.Reason == ResultReason.NoMatch)
                {
                    MessageBox.Show("NOMATCH: Speech could not be recognized.", "Easy Appointment", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else if (result.Reason == ResultReason.Canceled)
                {
                    var cancellation = CancellationDetails.FromResult(result);
                    // "CANCELED: Reason={cancellation.Reason}";

                    if (cancellation.Reason == CancellationReason.Error)
                    {
                        MessageBox.Show("CANCELED: " + cancellation.ErrorDetails, "Easy Appointment", MessageBoxButton.OK, MessageBoxImage.Error);
                        //Console.WriteLine($"CANCELED: ErrorDetails={cancellation.ErrorDetails}");
                        //Console.WriteLine($"CANCELED: Did you update the subscription info?");
                    }
                }
            }
            Action<TextBox, String, CheckBox, bool> updateAction = new Action<TextBox, string, CheckBox, bool>(UpdateTb);
            tb.Dispatcher.BeginInvoke(updateAction, tb, rlt, ckb, false); ;
        }

    }
}
