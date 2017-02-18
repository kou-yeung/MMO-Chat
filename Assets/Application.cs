
using UnityEngine;
using UnityEngine.SceneManagement;
using Network;
using Logger;

public class Application : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(this);
    }
    void Start ()
    {

        // ロケータ設定
        LoggerService.SetLocator(new UnityLogger());        // ログ出力
        SocketService.SetLocator(new TcpClientService());   // ソケット通信

        // ログインシーンへ遷移する
        SceneManager.LoadScene("Login");
    }

    void Update()
    {
        // ソケット通信の更新
        SocketService.Locator.Poll();
    }
}


class UnityLogger : ILoggerService
{
    public void Error(string message) { Debug.LogError(message); }
    public void Error(string format, params object[] args) { Debug.LogErrorFormat(format, args); }
    public void Log(string message) { Debug.Log(message); }
    public void Log(string format, params object[] args) { Debug.LogFormat(format, args); }
    public void Warning(string message) { Debug.LogWarning(message); }
    public void Warning(string format, params object[] args) { Debug.LogWarningFormat(format, args); }
}
