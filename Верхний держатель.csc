using OpenTK;
using System;
using System.Collections;
using System.Collections.Generic;
using GCodeGeneratorNet.Core;
using GCodeGeneratorNet.Core.GCodes;
using GCodeGeneratorNet.Core.Misc;
using GCodeGeneratorNet.Core.Geometry;

static float holesRadius = 2f;
static float holesDistance = 15;
static float outRadius = 5;
static float smallRadius = 3;
static float height = 15;
static float holeDistance = 30;
static float shaftLenght = 50;

public static Part25D Handle(Vector2 center1, bool down)
{
    var center2 = center1 + new Vector2(holesDistance, 0);
    var center3 = center1 + new Vector2(smallRadius, -height);
	var list = new List<IContourPart>();
	list.Add(new Arc(center1, outRadius, Math.PI / 2, -Math.PI / 2, RotateDirection.CCW));
	if(down)
	{
		list.Add(new Arc(center3, smallRadius, Math.PI, -0.7f , RotateDirection.CCW));
	}
	list.Add(new Arc(center2, outRadius, down?(-0.7f):0 - Math.PI / 2, Math.PI / 2, RotateDirection.CCW));
	
	return new Part25D(4,
		new Contour(list)
	, null,
	new []
	{
		new Contour(new Arc(center1, holesRadius, 0, 0, RotateDirection.CCW)),
		new Contour(new Arc(center2, holesRadius, 0, 0, RotateDirection.CCW)),
	});
}

public static Part25D Shaft(Vector2 center)
{
	return new Part25D(4, 
	new Contour(
		new Arc(center, outRadius, 0, Math.PI, RotateDirection.CCW),
		new ContourPoint(center + new Vector2(-outRadius,-shaftLenght)),
		new ContourPoint(center + new Vector2(outRadius,-shaftLenght))
		), null, 
		new []
	{
		new Contour(new Arc(center, holesRadius, 0, 0, RotateDirection.CCW)),
		new Contour(new Arc(center + new Vector2(0, -holeDistance), holesRadius, 0, 0, RotateDirection.CCW)),
	});
}


public static GScriptResult Generate()
{
    GCodeGenerator gcg = new GCodeGenerator();
    var parts = new List<Part25D>();
    parts.Add(Handle(new Vector2(), false));
    parts.Add(Handle(new Vector2(0, -15), true));
    parts.Add(Handle(new Vector2(0, -45), false));
    parts.Add(Handle(new Vector2(0, -60), true));
    parts.Add(Shaft(new Vector2(30, 0)));
    parts.Add(Shaft(new Vector2(45, 0)));
    foreach(var part in parts)
    {
    	gcg.Part25D(part);
    }
    gcg.GoToSafetyHeight();
    return new GScriptResult(parts, gcg.Codes);
}

