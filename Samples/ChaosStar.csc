using OpenTK;
using System;
using System.Collections;
using System.Collections.Generic;
using GCodeGeneratorNet.Core;
using GCodeGeneratorNet.Core.Geometry;
using GCodeGeneratorNet.Core.GCodes;
using GCodeGeneratorNet.Core.Misc;

static float materialHeight = 1.5f;
static float radius = 20;
static float arrowHeight = 0.3f;
static float arrowWidth = 0.15f;
static float arrowBaseWidth = 0.05f;
static float r1 = 0.59f;
static float r2 = r1 - arrowWidth / 2;

public static Part25D ChaosStar()
{
	var pb = new PartBuilder();
	var a1 = Math.Asin(arrowBaseWidth / r1);
	foreach(Angle angle in Angle.Angles(8))
    {
    	var mainVector = angle.HorizontalVector * radius;
    	var mp = mainVector.PerpendicularRight;
    	var arrowBase = mainVector * (1 - arrowHeight);
    	pb.AddArc(new Vector2(), radius * r1, 
    		angle - Math.PI / 4 + a1, angle - a1, RotateDirection.CCW);
    	pb.AddPoint(arrowBase + (mp * arrowBaseWidth));
    	pb.AddPoint(arrowBase + mp * arrowWidth);
    	pb.AddPoint(mainVector);
    	pb.AddPoint(arrowBase - mp * arrowWidth);
		pb.AddPoint(arrowBase - (mp * arrowBaseWidth));
    	
    }
    pb.CreateContour();
    float r3 = arrowBaseWidth / (float)Math.Sin(Math.PI / 8);
    foreach(Angle angle in Angle.Angles(8))
    {
    	pb.AddArc(new Vector2(), radius * r2, 
    		angle - Math.PI / 4 + a1, angle - a1, RotateDirection.CCW);
    	var a2 = angle - Math.PI / 8;
    	pb.AddPoint(a2.HorizontalVector * r3 * radius);
    	pb.CreateHole();
    }
    return pb.CreatePart(materialHeight);
}
    
public static GScriptResult Generate()
{
    GCodeGenerator gcg = new GCodeGenerator();
    gcg.HorizontalFeedRate = 500;
    gcg.ToolRadius = 0.01f;
    gcg.SafetyHeight = 2;
    gcg.VerticalStep = 1;
    gcg.MaterialHeight = 1;
    gcg.BridgeCount = 0;
    var parts = new List<Part25D>();
    parts.Add(ChaosStar());
    foreach(Part25D p in parts)
    {
    	gcg.Part25D(p);
    }
    gcg.GoToSafetyHeight();
    return new GScriptResult(parts, gcg.Codes);
}

