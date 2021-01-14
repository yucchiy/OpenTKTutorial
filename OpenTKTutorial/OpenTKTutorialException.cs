using System;

namespace OpenTKTutorial
{
    public class OpenTKTutorialException : Exception
    {
        public OpenTKTutorialException()
        {
        }

        public OpenTKTutorialException(string message) : base(message)
        {
        }

        public OpenTKTutorialException(string message, Exception inner) : base(message, inner)
        {
        }
   }
}