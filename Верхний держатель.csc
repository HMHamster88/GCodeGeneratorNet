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
static float shaftLenght = 48;
static float stopWidth = 8;
static float stopHeight = 6.8f;
static float t = 0.04f;

/*public static Part25D Handle(Vector2 center1)
{
	center1 = center1 + new Vector2(0, outRadius);
    var center2 = center1 + new Vector2(holesDistance, 0);
    var center3 = center1 + new Vector2(smallRadius, -height);
	var list = new List<IContourPart>();
	list.Add(new Arc(center1, outRadius, Math.PI / 2, -Math.PI / 2, RotateDirection.CCW));

	list.Add(new Arc(center2, outRadius, - Math.PI / 2, Math.PI / 2, RotateDirection.CCW));
	
	return new Part25D(4,
		new Contour(list)
	, null,
	new []
	{
		new Contour(new Arc(center1, holesRadius - t, 0, 0, RotateDirection.CCW)),
		new Contour(new Arc(center2, holesRadius - t, 0, 0, RotateDirection.CCW)),
	});
}*/

public static Part25D Shaft(Vector2 center)
{
	center = center + new Vector2(0, outRadius);
	return new Part25D(4, 
	new Contour(
		new ContourPoint(center + new Vector2(outRadius,shaftLenght)),
		new ContourPoint(center + new Vector2(-outRadius,shaftLenght)),
		new Arc(center, outRadius, Math.PI, 0, RotateDirection.CCW)
		), null, 
		new []
	{
		new Contour(new Arc(center, holesRadius + t, 0, 0, RotateDirection.CCW)),
		new Contour(new Arc(center + new Vector2(0, holeDistance), holesRadius + t, 0, 0, RotateDirection.CCW)),
	});
}

public static Part25D Support(Vector2 zero)
{
	float width = 20;
	float height = 50;
	float x1 = 5;
	float y1 = 10;
	float y2 = 15;
	float x2 = 2;
	float x3 = 5;
	float r1 = 1.25f;
	float x4 = 1.5f;
	return new Part25D(4, Contour.Rectangle(zero, new Vector2(width, height)), new[]
	{
		new Pocket(4 - 2.5f, 
		new ContourPoint(zero + new Vector2(0, y2)),
		new ContourPoint(zero + new Vector2(stopWidth, y2)),
		new ContourPoint(zero + new Vector2(stopWidth, stopHeight + y2)),
		new ContourPoint(zero + new Vector2(0, stopHeight + y2)))
	}, 
	new []
	{
		new Contour(new Arc(zero + new Vector2(x1, y1), holesRadius + t, 0, 0, RotateDirection.CCW)),
		//new Contour(new Arc(zero + new Vector2(x1, y1 + holeDistance), holesRadius + t, 0, 0, RotateDirection.CCW)),
	}
	);
}

public static Part25D Handle(Vector2 zero)
{
	float width = 20;
	float height = 25;
	return new Part25D(4, new Contour(
		new Arc(zero + new Vector2(outRadius, outRadius), outRadius, Math.PI, -Math.PI / 2, RotateDirection.CCW),
		new Arc(zero + new Vector2(width, outRadius), outRadius, -Math.PI/2, Math.PI/2, RotateDirection.CCW),
		new ContourPoint(zero + new Vector2(outRadius * 2, outRadius * 2)),
		new ContourPoint(zero + new Vector2(outRadius * 2, height)),
		new ContourPoint(zero + new Vector2(0, height))
	),
	null,
	new []
	{
		new Contour(new Arc(zero + new Vector2(width, outRadius), holesRadius - t, 0, 0, RotateDirection.CCW))
	}
	);
}

public static GScriptResult Generate()
{
    GCodeGenerator gcg = new GCodeGenerator();
    gcg.SafetyHeight = 4;
    gcg.MaterialHeight = 4;
    var parts = new List<Part25D>();
    /*for(int i = 0; i < 4;i++)
    {
    	parts.Add(Handle(new Vector2(0, 15 * i)));
    }
    parts.Add(Shaft(new Vector2(30, 0)));*/
    parts.Add(Support(new Vector2(0, 0)));
    parts.Add(Handle(new Vector2(25, 0)));
    parts.Add(Handle(new Vector2(25, 30)));
    foreach(var part in parts)
    {
    	gcg.Part25D(part);
    }
    gcg.GoToSafetyHeight();
    return new GScriptResult(parts, gcg.Codes);
}

