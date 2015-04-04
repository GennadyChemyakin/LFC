# LFC
Application for last.fm for Windows Phone

### Пример использования клиента

```c#
LfcAuth auth = new LfcAuth(username, password);
Client client = new Client(auth);
var response = client.userGetInfo("kiselevsergey");

```
