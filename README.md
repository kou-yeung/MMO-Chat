# MMO-Chat
MMO Server/Client Chat Sample  

# 構成
MMO
- Chat
  - Assets
  - ProjectSettings
  - Server

Assets & ProjectSettings は Unityプロジェクトディレクトリです  
Server は チャットサーバの VisualStudio プロジェクトを配置されます  
両方とも C# で書かれていて、共有したいロジックは Unity プロジェクト内に追加し  
チャットサーバは該当ファイルを「リンクとして追加」する  

# テスト手順
1.まず、チャットサーバの VisualStudioプロジェクトをビルドしアプリケーションを起動しておく  
2.Unityプロジェクトを開き「Play」ボタンで開始する  
3.UnityのGameシーンにログイン画面が表示される  
4.ID & Password (適当で) を入力し「Login」ボタンを押す  
5.ログイン成功したらチャットルーム(っぽい)画面に遷移できます  
  
※Windowsスタンドアロンを出力すると、複数人のチャットもテストできる  

# 技術詳細(チャレンジしたこと)
TcpClient と TcpListener を使って Server/Client 通信を行います。  
  
通信プロトコールは    
- [ データサイズ(int) | シリアライズされたデータ(可変長) ]  

↑のフォーマットで行う。  
シリアライザーは MessagePack を使用しています  
  
ProtoMaker.Pack(...) と ProtoMaker.Unpack(...) を提供され  
シリアライズ周りのコードを隠しました。  

Unity側では 初めてServiceLocatorを使って実装してみた。  
  
# 注意書き
このサンプルはとりあえず動いた状態になっています。（未リファクタリングです)

# TODO
- Server->Client への送信は TaskSend で行う
- プロトコールを自動生成する
- ~~AWSで動作検証したい~~ (Done : 2017/02/20)
- DBを使って実際の認証を行う
- リファクタリング

まだいろんなやりたいなぁ  
このリポジトリでやるかもしれないし  
一回捨てて書き直すかもしれませんけど。
