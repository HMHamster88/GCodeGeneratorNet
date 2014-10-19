using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCodeGeneratorNet.Core.GCodes
{
    public class GComment : IGCode
    {
        string comments;

        public string Comments
        {
            get
            {
                return comments;
            }
        }

        public Type Type
        {
            get { return GCodes.Type.G; }
        }

        public int Code
        {
            get { return 1000; }
        }

        public string Description
        {
            get { return "Comment"; }
        }

        public IEnumerable<OpenTK.Vector3> GetPoints(OpenTK.Vector3 initPos, bool absolute)
        {
            return new OpenTK.Vector3[0];
        }

        public GComment(string comments)
        {
            this.comments = comments;
        }

        public override string ToString()
        {
            return string.Format("({0})", Comments);;
        }
    }
}
