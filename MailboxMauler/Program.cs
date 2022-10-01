using MailKit;
using MailKit.Net.Imap;

string? imapHost;
do
{
    Console.Write("IMAP Host: ");
    imapHost = Console.ReadLine();
}
while (string.IsNullOrWhiteSpace(imapHost));

int imapPort;
string? sImapPort;
do
{
    Console.Write("IMAP Port (143/993): ");
    sImapPort = Console.ReadLine();
}
while (!int.TryParse(sImapPort, out imapPort) || (imapPort != 143 && imapPort != 993));

bool useSsl = imapPort == 993;
if (!useSsl)
    Console.WriteLine("WARNING: Connection is not secure");

string? username;
do
{
    Console.Write("Username: ");
    username = Console.ReadLine();
}
while (string.IsNullOrWhiteSpace(username));

string? password;
do
{
    Console.Write("Password: ");
    password = GetPassword();
}
while (string.IsNullOrWhiteSpace(password));
Console.WriteLine();

int maxAge;
string? sMaxAge;
do
{
    Console.Write("Delete emails older than x days: ");
    sMaxAge = Console.ReadLine();
} while (!int.TryParse(sMaxAge, out maxAge));


using (var client = new ImapClient())
{
    client.Connect(imapHost, imapPort, useSsl);

    client.Authenticate(username, password);

    // the Inbox folder is always available on all IMAP servers
    IMailFolder inbox = client.Inbox;
    inbox.Open(FolderAccess.ReadWrite);

    Console.CancelKeyPress += delegate
    {
        Console.WriteLine("Ctrl + C captured. Disconnecting client and logging out.");
        inbox.Close(expunge: true);
        client.Disconnect(quit: true);
        client.Dispose();
    };

    IMailFolder trash = client.GetFolder("Trash");
    if (!trash.Exists)
    {
        Console.WriteLine("Trash not found.");
        return;
    }

    Console.WriteLine($"Total messages: {inbox.Count}");

    inbox.Open(FolderAccess.ReadWrite);

    for (int i = 0; i < inbox.Count; i++)
    {
        var message = inbox.GetMessage(i);

        TimeSpan messageAge = DateTime.UtcNow - message.Date.UtcDateTime;
        if (messageAge.Days > maxAge)
        {
            Console.WriteLine($"Deleting: Date: {message.Date}, Subject: {message.Subject}");

            inbox.MoveTo(i, trash);
        }
        else
        {
            Console.WriteLine($"Skipping: Date: {message.Date}, Subject: {message.Subject}");
        }
    }

    inbox.Close(expunge: true);
    client.Disconnect(quit: true);
}

static string GetPassword()
{
    string password = string.Empty;
    ConsoleKey key;
    do
    {
        var keyInfo = Console.ReadKey(intercept: true);
        key = keyInfo.Key;

        if (key == ConsoleKey.Backspace && password.Length > 0)
        {
            Console.Write("\b \b");
            password = password[0..^1];
        }
        else if (!char.IsControl(keyInfo.KeyChar))
        {
            Console.Write("*");
            password += keyInfo.KeyChar;
        }
    } while (key != ConsoleKey.Enter);

    return password;
}