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
using GCodeGeneratorNet.Core.Geometry;
using GCodeGeneratorNet.Core.GCodes;
using GCodeGeneratorNet.Core.Misc;

public static GScriptResult Generate()
{
    GCodeGenerator gcg = new GCodeGenerator();
    gcg.HorizontalFeedRate = 500;
    gcg.ToolRadius = 1f;
    gcg.SafetyHeight = 2;
    gcg.VerticalStep = 2;
    gcg.MaterialHeight = 4;
    gcg.BridgeCount = 0;
    
    var parts = new List<Part25D>();
    var pb = new PartBuilder();
    pb.AddCircle(new Vector2(10, 10), 10);
    pb.CreateContour();
    pb.AddCircle(new Vector2(10, 10), 5);
    pb.CreateHole();
    parts.Add(pb.CreatePart(gcg.MaterialHeight));
    return new GScriptResult(gcg, parts);
}";
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
