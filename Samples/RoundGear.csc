using OpenTK;
using System;
using System.Collections;
using System.Collections.Generic;
using GCodeGeneratorNet.Core;
using GCodeGeneratorNet.Core.GCodes;
using GCodeGeneratorNet.Core.Misc;
using GCodeGeneratorNet.Core.Geometry;

public static Contour Gear(Vector2 center, float toothRadius, int toothCount)
{
	PartBuilder pb = new PartBuilder();
	Angle betta = Math.PI * 2 / toothCount;
	float radius = (float)((toothRadius * 2) / (2 * Math.Sin(betta/4)));
	Angle da = Math.PI/2 + betta/2;
	foreach(Angle angle in Angle.Angles(toothCount))
	{
		pb.AddArc(angle.HorizontalVector * radius, toothRadius, angle - Math.PI/2, angle + Math.PI/2, RotateDirection.CCW);
		var angle2 = angle + betta / 2;
		pb.AddArc(angle2.HorizontalVector * radius, toothRadius, angle2 - da, angle2 + da, RotateDirection.CW);
	}
	return pb.CreateContour();
}

public static GScriptResult Generate()
{
    GCodeGenerator gcg = new GCodeGenerator();
    gcg.SafetyHeight = 4;
    gcg.MaterialHeight = 4;
    gcg.HorizontalFeedRate = 200;
    gcg.VerticalStep = 1;
    var parts = new List<Part25D>();
    
    PartBuilder pb = new PartBuilder();
    pb.SetContour(Gear(new Vector2(), 3, 8));
    
    /*foreach(Angle angle in Angle.Angles(6))
	{
		pb.AddCircle(angle.HorizontalVector * 27, 10);
		pb.CreateHole();		
	}*/

	pb.AddCircle(new Vector2(), 2);
	pb.CreateHole();
    parts.Add(pb.CreatePart(4));


    foreach(var part in parts)
    {
    	gcg.Part25D(part);
    }
    gcg.GoToSafetyHeight();
    return new GScriptResult(parts, gcg.Codes);
}

