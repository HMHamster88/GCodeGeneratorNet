using GCodeGeneratorNet.Core.GCodes;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCodeGeneratorNet.Core
{
    public class GCodeGenerator
    {
        List<IGCode> codes = new List<IGCode>();
        public IEnumerable<IGCode> Codes
        {
            get
            {
                return codes;
            }
        }

        public Vector3 CurrentPosition { get; set; }

        public float Tolerance { get; set; }
        public float ToolRadius { get; set; }

        public float MaterialHeight { get; set; }
        public float SafetyHeight { get; set; }

        public float RapidMoveRate { get; set; }
        public float FeedRate { get; set; }

        public float VerticalRate { get; set; }

        public GCodeGenerator()
        {
            MaterialHeight = 8;
            SafetyHeight = 4;
            GoToSafetyHeight();
        }

        public void GoToSafetyHeight()
        {

            
            codes.Add(new GMOVE(true, null, null, MaterialHeight + SafetyHeight));
        }

        public void RapidMoveTo(Vector2 pos)
        {
            codes.Add(new GMOVE(true, pos.X, pos.Y, null));
        }
    }
}
