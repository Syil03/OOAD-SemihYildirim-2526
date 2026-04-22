using WpfEscapeGame.Enums;
using System;
using System;
using System.Collections.Generic;
using System.Text;

namespace WpfEscapeGame.Classes
{
    public static class RandomMessageGenerator
    {
        private static Random rnd = new Random();

        private static string[] foundMessages = {
            "Oh nice, I found something!",
            "Well well well, what do we have here...",
            "Interesting discovery!"
        };

        private static string[] failMessages = {
            "That doesn't seem to work.",
            "Nope, not happening.",
            "Hmm, that's not right."
        };

        private static string[] infoMessages = {
            "I see nothing special.",
            "Just an ordinary object.",
            "Nothing unusual here."
        };

        public static string GetRandomMessage(MessageType t)
        {
            switch (t)
            {
                case MessageType.Found: return foundMessages[rnd.Next(foundMessages.Length)];
                case MessageType.Fail: return failMessages[rnd.Next(failMessages.Length)];
                default: return infoMessages[rnd.Next(infoMessages.Length)];
            }
        }
    }
}
