using OpenTK;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using GCodeGeneratorNet.Core;
using GCodeGeneratorNet.Core.GCodes;
using GCodeGeneratorNet.Core.Misc;

public static IEnumerable<IGCode> Generate()
{
    GCodeGenerator gcg = new GCodeGenerator();
    var myMatrix = new Matrix();
    myMatrix.Translate(0, -30, MatrixOrder.Append);
    gcg.Text("132abc", FontFamily.GenericSansSerif, 0, 20, myMatrix, 0, 0.1f);
    return gcg.Codes;
}

