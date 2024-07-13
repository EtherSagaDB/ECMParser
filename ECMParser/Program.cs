using System;
using System.Text;

namespace MyApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            string FileContents = File.ReadAllText(args[0], Encoding.GetEncoding(936));

            String[] FileContentsArray = FileContents.Split("\r\n");
            String[] NewFileContentsArray = new String[FileContentsArray.Length];
            Int32 CurrentScriptLine = 0;
            Script script = new Script();

            MkDir("./Out");
            MkDir($"./Out/{Path.GetFileName(args[0]).Replace(".ecm", "")}");
            MkDir($"./Out/{Path.GetFileName(args[0]).Replace(".ecm", "")}/Scripts");

            for (Int32 i = 0; i < FileContentsArray.Length; i++)
            {
                if (string.IsNullOrEmpty(FileContentsArray[i])) {
                    continue;
                }
                if (FileContentsArray[i].Contains("ScriptCount"))
                {
                    // Console.WriteLine(i + 1);
                    Int32 ScriptCount = Int32.Parse(FileContentsArray[i].Split(": ")[1]);
                    // Console.WriteLine(ScriptCount);
                }
                if (FileContentsArray[i].Contains("id:"))
                {
                    // Console.WriteLine(i + 1);
                    Int32 ScriptID = Int32.Parse(FileContentsArray[i].Split(": ")[1]);
                    script.ID = ScriptID;
                    // Console.WriteLine(ScriptID);
                }
                if (FileContentsArray[i].Contains("ScriptLines:"))
                {
                    // Console.WriteLine(i + 1);
                    Int32 ScriptLines = Int32.Parse(FileContentsArray[i].Split(": ")[1]);
                    script.Lines = ScriptLines;
                    // Console.WriteLine(ScriptLines);
                }

                if (IsBase64String(FileContentsArray[i]))
                {
                    if (CurrentScriptLine > script.Lines)
                    {
                        CurrentScriptLine = 0;
                    }
                    CurrentScriptLine++;
                    // Console.WriteLine(FileContentsArray[i]);
                    byte[] data = Convert.FromBase64String(FileContentsArray[i]);
                    String NewTextLine = StringParser(data);
                    File.AppendAllText(Path.Combine($"./Out/{Path.GetFileName(args[0]).Replace(".ecm", "")}/Scripts/", $"Script-{script.ID}.lua"), NewTextLine);
                    NewFileContentsArray[i] = $"DecodedScriptFile: {script.ID}";
                    continue;
                }
                NewFileContentsArray[i] = FileContentsArray[i];
            }
            File.WriteAllLines(Path.Combine($"./Out/{Path.GetFileName(args[0]).Replace(".ecm", "")}/", $"{Path.GetFileName(args[0]).Replace(".ecm", "")}.txt"), NewFileContentsArray);
        }

        static bool IsBase64String(string base64)
        {
            Span<byte> buffer = new Span<byte>(new byte[base64.Length]);
            return Convert.TryFromBase64String(base64, buffer, out int bytesParsed);
        }

        static string StringParser(byte[] data)
        {
            string text = Encoding.GetEncoding(936).GetString(data).Replace("\0", "");
            return text;
        }

        static void MkDir(String path)
        {
            try
            {
                // Determine whether the directory exists.
                if (Directory.Exists(path) && path != "./Out")
                {
                    Directory.Delete(path, true);
                    return;
                }

                // Try to create the directory.
                DirectoryInfo di = Directory.CreateDirectory(path);
                // Console.WriteLine("The directory was created successfully at {0}.", Directory.GetCreationTime(path));
            }
            catch (Exception e)
            {
                Console.WriteLine("The process failed: {0}", e.ToString());
            }
            finally { }
        }
    }

    public class Script
    {
        public Int32 ID { get; set; }
        public Int32 Lines { get; set; }
        public String[]? TextLines { get; set; }
    }
}