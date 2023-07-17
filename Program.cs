using BencodeNET.Parsing;
using BencodeNET.Torrents;
// we need only .torrent files
const string FILE_MASK = "*.torrent";
// directory where qBittorrent saving .torrent files (%LOCALAPPDATA%/qBittorrent/BT_backup)
// if have problems, insert your path
string DIRECTORY = $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}/qBittorrent/BT_backup/";
// get path of all .torrent files
string[] files = Directory.GetFiles(DIRECTORY, FILE_MASK);
// class for parsing .torrent file data
BencodeParser parser = new BencodeParser();
// save torrent urls for detecting duplicates later
List<string> torrentUrl = new List<string>();
foreach (string file in files)
{
    Torrent torrent = parser.Parse<Torrent>(file);
    torrentUrl.Add(torrent.Comment);
}
Console.WriteLine($"Found {torrentUrl.Count} .torrent files");
// count and select torrents url duplications
var query = from t in torrentUrl
            group t by t into g
            let count = g.Count()
            orderby count descending
            where count > 1
            select new { Url = g.Key, Count = count };
if (query.Count() > 0)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"Found {query.Count()} duplications");
    Console.ForegroundColor = ConsoleColor.Yellow;
    foreach (var x in query)
    {
        Console.WriteLine("URL: " + x.Url + " Duplicates: " + x.Count);
    }
}
else
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("Duplications not found");
}
Console.ReadLine();