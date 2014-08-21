using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        private string text =
@"
using System;
using System.Collections;
using System.Collections.Generic;
using GCodeGeneratorNet.Core.GCodes;

public static IEnumerable<IGCode> Generate()
{
    yield return new G90();
    yield return new G00(0, 2);
    yield return new G02(0, -2, 0, 0, -2);
}

";

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
    }
}
