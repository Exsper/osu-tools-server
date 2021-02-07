using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Threading;
using System.IO;
using System.Collections.Specialized;

namespace osu_tools_server
{
    class Api
    {
        readonly static string PPCALPATH = Environment.CurrentDirectory + "\\PerformanceCalculator\\PerformanceCalculator.exe";
        readonly static string BEATMAP_FOLDER = Environment.CurrentDirectory + "\\Beatmap\\";
        readonly static string BEATMAP_URL = "https://osu.ppy.sh/osu/";

        static string CmdCall(string s)
        {
            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo.FileName = PPCALPATH;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.Arguments = s;
            p.Start();
            p.StandardInput.WriteLine(s);
            p.StandardInput.WriteLine("Exit");
            string str = p.StandardOutput.ReadToEnd();
            p.WaitForExit();
            p.Close();
            return str;
        }

        static string DownloadBeatmap(string bid, string filepath)
        {
            WebClient webClient = new WebClient();
            webClient.DownloadFile(BEATMAP_URL + bid, filepath);
            return filepath;
        }

        static string GetBeatmapPath(string bid)
        {
            string filepath = BEATMAP_FOLDER + bid + ".osu";
            if (File.Exists(filepath)) return filepath;
            else return DownloadBeatmap(bid, filepath);
        }

        static string getBeatmapText(string bid)
        {
            string filepath = BEATMAP_FOLDER + bid + ".osu";
            if (!File.Exists(filepath)) filepath = DownloadBeatmap(bid, filepath);
            string fileData = File.ReadAllText(filepath);
            return fileData;
        }

        static string GetModsCommand(string mods)
        {
            int raw_mods = int.Parse(mods);
            List<string> modsArr = new List<string>();
            if ((raw_mods & 1) > 0) modsArr.Add("NF");
            if ((raw_mods & 2) > 0) modsArr.Add("EZ");
            if ((raw_mods & 4) > 0) modsArr.Add("TD");
            if ((raw_mods & 8) > 0) modsArr.Add("HD");
            if ((raw_mods & 16) > 0) modsArr.Add("HR");
            if ((raw_mods & 32) > 0) modsArr.Add("SD");
            if ((raw_mods & 64) > 0) modsArr.Add("DT");
            if ((raw_mods & 128) > 0) modsArr.Add("Relax");
            if ((raw_mods & 256) > 0) modsArr.Add("HT");
            if ((raw_mods & 512) > 0) { modsArr.Add("NC"); modsArr.Remove("DT"); }
            if ((raw_mods & 1024) > 0) modsArr.Add("FL");
            if ((raw_mods & 2048) > 0) modsArr.Add("Auto");
            if ((raw_mods & 4096) > 0) modsArr.Add("SO");
            if ((raw_mods & 8192) > 0) modsArr.Add("AP");
            if ((raw_mods & 16384) > 0) { modsArr.Add("PF"); modsArr.Remove("SD"); };
            if ((raw_mods & 32768) > 0) modsArr.Add("4K");
            if ((raw_mods & 65536) > 0) modsArr.Add("5K");
            if ((raw_mods & 131072) > 0) modsArr.Add("6K");
            if ((raw_mods & 262144) > 0) modsArr.Add("7K");
            if ((raw_mods & 524288) > 0) modsArr.Add("8K");
            if ((raw_mods & 1048576) > 0) modsArr.Add("FI");
            if ((raw_mods & 2097152) > 0) modsArr.Add("RD");
            if ((raw_mods & 4194304) > 0) modsArr.Add("Cinema");
            if ((raw_mods & 8388608) > 0) modsArr.Add("Target");
            if ((raw_mods & 16777216) > 0) modsArr.Add("9K");
            if ((raw_mods & 33554432) > 0) modsArr.Add("KeyCoop");
            if ((raw_mods & 67108864) > 0) modsArr.Add("1K");
            if ((raw_mods & 134217728) > 0) modsArr.Add("3K");
            if ((raw_mods & 268435456) > 0) modsArr.Add("2K");
            if ((raw_mods & 536870912) > 0) modsArr.Add("ScoreV2");
            if ((raw_mods & 1073741824) > 0) modsArr.Add("MR");
            string mCommand = "";
            foreach (string modString in modsArr)
            {
                mCommand += " -m " + modString;
            }
            return mCommand;
        }
        static string GetPPcalCommand(NameValueCollection urlquery)
        {
            string command = "simulate";
            string bid = urlquery.Get("id");
            if (bid == null) throw new Exception("param id is necessary");
            string beatmappath = GetBeatmapPath(bid);
            beatmappath = beatmappath.Replace("\\", "\\\\");
            string mode = urlquery.Get("m");
            if (mode == null) mode = "0";
            switch (mode)
            {
                case "0":
                    {
                        string acc = urlquery.Get("acc");
                        string combo = urlquery.Get("combo");
                        string mods = urlquery.Get("mods");
                        string miss = urlquery.Get("miss");
                        string count50 = urlquery.Get("50");
                        string count100 = urlquery.Get("100");
                        command += " osu";
                        if (acc != null) command += " -a " + acc;
                        if (combo != null) command += " -c " + combo;
                        if (miss != null) command += " -X " + miss;
                        if (count50 != null) command += " -M " + count50;
                        if (count100 != null) command += " -G " + count100;
                        if (mods != null) command += GetModsCommand(mods);
                        break;
                    }
                case "1":
                    {
                        string acc = urlquery.Get("acc");
                        string combo = urlquery.Get("combo");
                        string mods = urlquery.Get("mods");
                        string miss = urlquery.Get("miss");
                        string count100 = urlquery.Get("100");
                        command += " taiko";
                        if (acc != null) command += " -a " + acc;
                        if (combo != null) command += " -c " + combo;
                        if (miss != null) command += " -X " + miss;
                        if (count100 != null) command += " -G " + count100;
                        if (mods != null) command += GetModsCommand(mods);
                        break;
                    }
                case "2":
                    {
                        string acc = urlquery.Get("acc");
                        string combo = urlquery.Get("combo");
                        string mods = urlquery.Get("mods");
                        string miss = urlquery.Get("miss");
                        string count50 = urlquery.Get("50");
                        string count100 = urlquery.Get("100");
                        command += " catch";
                        if (acc != null) command += " -a " + acc;
                        if (combo != null) command += " -c " + combo;
                        if (miss != null) command += " -X " + miss;
                        if (count50 != null) command += " -T " + count50;
                        if (count100 != null) command += " -D " + count100;
                        if (mods != null) command += GetModsCommand(mods);
                        break;
                    }
                case "3":
                    {
                        string score = urlquery.Get("score");
                        string mods = urlquery.Get("mods");
                        command += " mania";
                        if (score != null) command += " -s " + score;
                        if (mods != null) command += GetModsCommand(mods);
                        break;
                    }
            }
            command += " -j ";
            command += beatmappath;
            return command;
        }

        static string GetDiffcalCommand(NameValueCollection urlquery)
        {
            string command = "difficulty";
            string bid = urlquery.Get("id");
            if (bid == null) throw new Exception("param id is necessary");
            string beatmappath = GetBeatmapPath(bid);
            beatmappath = beatmappath.Replace("\\", "\\\\");
            string mode = urlquery.Get("m");
            if (mode != null) command += " -r:" + mode;
            string mods = urlquery.Get("mods");
            if (mods != null) command += GetModsCommand(mods);
            command += " " + beatmappath;
            return command;
        }

        static string GetDiffResultInfo(string diffResult)
        {
            string[] a = diffResult.Split("│");
            int length = a.Length;
            string[] titles = a.Skip(1).Take(length / 2).ToArray();
            for (int i = 0; i < titles.Length; i++)
            {
                titles[i] = titles[i].Split("║")[0];
            }
            string[] valueStrings = a.Skip(length / 2 + 1).ToArray();
            float[] values = new float[valueStrings.Length];
            for (int i = 0; i < valueStrings.Length; i++)
            {
                values[i] = float.Parse(valueStrings[i].Split("║")[0]);
            }
            string response = "{";
            for (int i = 0; i < valueStrings.Length; i++)
            {
                if (i < valueStrings.Length - 1) response += "\"" + titles[i] + "\": " + values[i] + ", ";
                else response += "\"" + titles[i] + "\": " + values[i];
            }
            response += "}";
            return response;
        }

        public static string GetResponse(string urlpath, NameValueCollection urlquery)
        {
            if (urlpath == "/getBeatmap")
            {
                string bid = urlquery.Get("id");
                if (bid == null) throw new Exception("param id is necessary");
                return GetBeatmapPath(bid);
            }
            if (urlpath == "/getBeatmapText")
            {
                string bid = urlquery.Get("id");
                if (bid == null) throw new Exception("param id is necessary");
                return getBeatmapText(bid);
            }
            else if (urlpath == "/cal")
            {
                string command = GetPPcalCommand(urlquery);
                string response = CmdCall(command);
                return response;
            }
            else if (urlpath == "/difficulty")
            {
                string command = GetDiffcalCommand(urlquery);
                string response = CmdCall(command);
                return GetDiffResultInfo(response);
            }
            else throw new Exception("unknown command");
        }
    }
}
