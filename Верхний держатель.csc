using OpenTK;
using System;
using System.Collections;
using System.Collections.Generic;
using GCodeGeneratorNet.Core;
using GCodeGeneratorNet.Core.GCodes;
using GCodeGeneratorNet.Core.Misc;
using GCodeGeneratorNet.Core.Geometry;

public static IEnumerable<IGCode> Generate()
{
    GCodeGenerator gcg = new GCodeGenerator();
    float holesRadius = 2.5f;
    float holesDistance = 15;
    float outRadius = 5;
    float smallRadius = 3;
    float height = 15;
    var center1 = new Vector2();
    var center2 = center1 + new Vector2(holesDistance, 0);
    var center3 = center1 + new Vector2(smallRadius, -height);
	var part = new Part25D(8,
		new Contour(new IContourPart[] {
			new Arc(center1, outRadius, Math.PI / 2, -Math.PI / 2, RotateDirection.CCW),
			new Arc(center3, smallRadius, Math.PI, 0 , RotateDirection.CCW),
			new Arc(center2, outRadius, -Math.PI / 2, Math.PI / 2, RotateDirection.CCW),
		})
	, null,
	new Contour[]
	{
		new Contour(new IContourPart[] { new Arc(center1, holesRadius, 0, 0, RotateDirection.CCW)}),
		new Contour(new IContourPart[] { new Arc(center2, holesRadius, 0, 0, RotateDirection.CCW)}),
	});
    gcg.Part25D(part);
    gcg.GoToSafetyHeight();
    return gcg.Codes;
}

