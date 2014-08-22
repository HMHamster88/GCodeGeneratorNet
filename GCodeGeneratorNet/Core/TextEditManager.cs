using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCodeGeneratorNet.Core
{
    public class TextEditManager : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        void notifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private string text;

        public string Text
        {
            get
            {
                return text;
            }
            set
            {
                text = value;
                notifyPropertyChanged("Text");
            }
        }

        private string filepath;

        public string FilePath
        {
            get
            {
                return filepath;
            }
            set
            {
                filepath = value;
                Properties.Settings.Default.LastFile = value;
                notifyPropertyChanged("FilePath");
            }
        }

        public void Save(string path)
        {
            File.WriteAllText(path, Text);
            this.FilePath = path;
        }
        
        public void Open(string path)
        {
            Text = File.ReadAllText(path);
            this.FilePath = path;
        }

        public void New()
        {
            Text =
                @"using OpenTK;
using System;
using System.Collections;
using System.Collections.Generic;
using GCodeGeneratorNet.Core;
using GCodeGeneratorNet.Core.GCodes;
using GCodeGeneratorNet.Core.Misc;

public static IEnumerable<IGCode> Generate()
{
    GCodeGenerator gcg = new GCodeGenerator();
    return gcg.Codes;
}

";
            FilePath = null;
        }

        public TextEditManager()
        {
            FilePath = Properties.Settings.Default.LastFile;
            if(!string.IsNullOrEmpty(FilePath) && File.Exists(FilePath))
            {
                Open(FilePath);
            }
            else
            {
                New();
            }
        }
    }
}
