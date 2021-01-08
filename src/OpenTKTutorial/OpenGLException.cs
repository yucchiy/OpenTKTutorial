using System;

namespace OpenTKTutorial
{
    public class OpenGLException : Exception
    {
        public OpenGLException()
        {
        }

        public OpenGLException(string message) : base(message)
        {
        }

        public OpenGLException(string message, Exception inner) : base(message, inner)
        {
        }
   }
}