using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

/*
 * Author: Vincent Qiu
 * Date: 2021.04
 */

async Task<string> GetRemoteContent()
{
    using var httpClient = new HttpClient();
    return await httpClient.GetStringAsync("https://cdn.jsdelivr.net/gh/521xueweihan/GitHub520@main/hosts");
}

const string title = "Github Hosts Modifier by Vincent Qiu.";
var systemHosts = $@"{Environment.SystemDirectory}\drivers\etc\hosts";
var splitLine = string.Join(string.Empty, Enumerable.Repeat("=", title.Length));
Console.WriteLine(title);
Console.WriteLine("Hosts source credits to 521xueweihan.");
Console.WriteLine("For Windows only.");
Console.WriteLine(splitLine);
try
{
    if (System.IO.File.GetAttributes(systemHosts).ToString().IndexOf("ReadOnly", StringComparison.Ordinal) != -1)
    {
        File.SetAttributes(systemHosts, FileAttributes.Normal);
    }
}
catch (Exception ex)
{
    Console.WriteLine($@"Error! {ex.Message}");
    Console.WriteLine(
        "Make sure you have modified the hosts file permissions with NOT readonly and users can writing.");
    Console.WriteLine("Or you can simply mouse right click and 'runs as Administrator'.[Windows]");
    // Console.WriteLine($@"Path: {systemHosts}");
    Console.ReadLine();
    return -3;
}

Console.WriteLine("Loading remote github hosts content...");
var contentTask = GetRemoteContent();
// Console.WriteLine("Content as below...");
// Console.WriteLine(contentTask.Result);
Console.WriteLine(splitLine);
var content = contentTask.Result;
const string hostSectionStartingText = "# GitHub520 Host Start";
const string hostSectionEndingText = "# GitHub520 Host End";
if (!content.Contains(hostSectionStartingText) || !content.Contains(hostSectionEndingText))
{
    Console.WriteLine("Remote github content error with begin and ending section text!");
    Console.ReadLine();
    return -2;
}

if (!File.Exists(systemHosts))
{
    Console.WriteLine($@"Can not find the hosts file at {systemHosts}!");
    Console.ReadLine();
    return -1;
}

Console.WriteLine("Staring merging...");
var hostsContent = File.ReadAllLines(systemHosts);
var newHosts = new StringBuilder();
var skipping = false;
foreach (var line in hostsContent)
{
    if (line.Contains(hostSectionStartingText))
        skipping = true;
    if (!skipping)
        newHosts.AppendLine(line);
    else if (line.Contains(hostSectionEndingText))
        skipping = false;
}

newHosts.Append(content);
Console.WriteLine(splitLine);
Console.WriteLine("Now writing to system Hosts file...");
try
{
    File.WriteAllText(systemHosts, newHosts.ToString());
    Console.WriteLine("Done!");
    Console.ReadLine();
}
catch (Exception ex)
{
    Console.WriteLine($@"Error! {ex.Message}");
    Console.WriteLine(
        "Make sure you have modified the hosts file permissions with NOT readonly and users can writing.");
    Console.WriteLine("Or mouse right click and 'runs as Administrators'.");
    Console.WriteLine($@"Path: {systemHosts}");
    Console.ReadLine();
}

return 0;