using System.Configuration;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text.RegularExpressions;
using System.Timers;
using Timer = System.Timers.Timer;

namespace Remover
{
    public partial class Service : ServiceBase
    {
        Config config;

        Timer timer;

        FileInfo[] files;

        int tick;
        int timeFormat;
        int tmp;

        string[] folders;
        string[] types;

        int[] limits;
        int[] intervals;
        int[] tmpIntervals;

        bool stop;

        public Service()
        {
            InitializeComponent();

            stop = false;

            tick = 60;
            timeFormat = 60;
            timer = new Timer(tick * 1000);

        }

        protected override void OnStart(string[] args)
        {   
            config = (Config)ConfigurationManager.GetSection("Config");

            tmp = config.Settings.Count;
            folders = new string[tmp];
            types = new string[tmp];
            limits = new int[tmp];
            intervals = new int[tmp];
            tmpIntervals = new int[tmp];

            tmp = 0;
            foreach (ConfigElement item in config.Settings)
            {
                folders[tmp] = item.Path;
                limits[tmp] = item.Limit;
                intervals[tmp] = item.Interval * timeFormat;
                types[tmp] = item.Type;
                tmp++;
            }

            Array.Copy(intervals, 0, tmpIntervals, 0, intervals.Length);

            timer.Enabled = true;
            timer.Elapsed += Timer_Elapsed;
        }

        protected override void OnStop()
        {
            stop = true;
            timer.Enabled = false;
        }


        protected override void OnShutdown()
        {
            OnStop();
            Dispose();
        }


        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            for (int i = 0; i < intervals.Length; i++)
            {
                if (stop) { break; }

                tmpIntervals[i]--;

                if (tmpIntervals[i] == 0)
                {
                    GetFiles(folders[i], types[i], out files);
                    Remove(ref files, ref limits[i]);
                    tmpIntervals[i] = intervals[i];
                }
            }
        }


        public void GetFiles(string folder, string type, out FileInfo[] files)
        {
            files = null;
            DirectoryInfo di = new DirectoryInfo(folder);

            if (di.Exists)
            {
                files = di.GetFiles()
                                   .Where(file => Regex.IsMatch(Path.GetExtension(file.Name), $@"^.({type})$"))
                                   .OrderBy(file => file.CreationTime)
                                   .ToArray();
            }
        }

        public void Remove(ref FileInfo[] files, ref int limit)
        {
            int numOfFiles = files.Length;
            if (numOfFiles > limit)
            {
                int overLimit = numOfFiles - limit;
                for (int i = 0; i < overLimit; i++)
                {
                    try
                    {
                        files[i].Delete();
                    }
                    catch { continue; };
                }
            }
        }
    }
}




