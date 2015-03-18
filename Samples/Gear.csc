using OpenTK;
using System;
using System.Collections;
using System.Collections.Generic;
using GCodeGeneratorNet.Core;
using GCodeGeneratorNet.Core.GCodes;
using GCodeGeneratorNet.Core.Misc;
using GCodeGeneratorNet.Core.Geometry;

public static Contour Gear(Vector2 center, float height, float toothBase, float toothM, int toothCount)
{
	PartBuilder pb = new PartBuilder();
	Angle betta = Math.PI * 2 / toothCount;
	Angle toothBaseAngle = betta * toothM;
	Angle toothTopAngle = betta - toothBaseAngle;
	
	float radius = (float)((toothBase * 2) / (2 * Math.Sin(toothBaseAngle/2)));
	
	float toothTop = (float)(2 * radius * Math.Sin(toothTopAngle / 2));
	
	Angle baseDa = toothBaseAngle / 2;
	
	foreach(Angle angle in Angle.Angles(toothCount))
	{
		pb.AddPoint((angle - baseDa).HorizontalVector * radius);
		
		pb.AddPoint(angle.HorizontalVector * (radius + height) + (angle - Math.PI / 2).HorizontalVector * toothTop / 2 );
		pb.AddPoint(angle.HorizontalVector * (radius + height) + (angle + Math.PI / 2).HorizontalVector * toothTop / 2 );
		
		pb.AddPoint((angle + baseDa).HorizontalVector * radius);
	}
	return pb.CreateContour();
}

public static GScriptResult Generate()
{
    GCodeGenerator gcg = new GCodeGenerator();
    gcg.SafetyHeight = 4;
    gcg.MaterialHeight = 4;
    var parts = new List<Part25D>();
    
    PartBuilder pb = new PartBuilder();
    pb.SetContour(Gear(new Vector2(), 4, 4, 0.7f, 48));
    
    foreach(Angle angle in Angle.Angles(6))
	{
		pb.AddCircle(angle.HorizontalVector * 55, 25);
		pb.CreateHole();		
	}

    parts.Add(pb.CreatePart(4));


    foreach(var part in parts)
    {
    	gcg.Part25D(part);
    }
    gcg.GoToSafetyHeight();
    return new GScriptResult(parts, gcg.Codes);
}

