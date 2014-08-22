using OpenTK;
using System;
using System.Collections;
using System.Collections.Generic;
using GCodeGeneratorNet.Core;
using GCodeGeneratorNet.Core.GCodes;
using GCodeGeneratorNet.Core.Misc;

public static IEnumerable<IGCode> Generate()
{
    GCodeGenerator gcg = new GCodeGenerator();
    gcg.RapidMoveTo(new Vector2(12, 13));
    return gcg.Codes;
}

