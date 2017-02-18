// ログ出力のインタフェース

namespace Logger
{
    interface ILoggerService
    {
        void Log(string message);
        void Log(string format, params object[] args);

        void Warning(string message);
        void Warning(string format, params object[] args);

        void Error(string message);
        void Error(string format, params object[] args);
    }

    class LoggerService : ServiceLocator<ILoggerService> { }
}
