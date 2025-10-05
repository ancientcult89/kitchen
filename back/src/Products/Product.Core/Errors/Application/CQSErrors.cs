using Primitives;

namespace Products.Core.Errors.Application
{
    public static class CQSErrors
    {
        public static Error IncorrectCommand()
        {
            return new Error("command.is.incorrect", "The command is incorrect");
        }
    }
}
