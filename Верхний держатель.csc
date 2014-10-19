using OpenTK;
using System;
using System.Collections;
using System.Collections.Generic;
using GCodeGeneratorNet.Core;
using GCodeGeneratorNet.Core.GCodes;
using GCodeGeneratorNet.Core.Misc;

public static void CreateHandler(GCodeGenerator gcg, Vector2 center1, bool bottomPart)
{
	float holesRadius = 2.5f;
    float holesDistance = 15;
    float outRadius = 5;
    float smallRadius = 3;
    float height = 15;
    var center2 = center1 + new Vector2(holesDistance, 0);
    var center3 = center1 + new Vector2(smallRadius, -height);
    gcg.RoundHole(center1, holesRadius);
    gcg.RoundHole(center2, holesRadius);
    bool firstStep = true;
    double startAngle = -Math.Asin(gcg.ToolRadius / (outRadius + gcg.ToolRadius)) -Math.PI / 2;
    foreach(float z in gcg.VerticalRange)
    {
    	var start = gcg.HorizontalArcStart(center1, outRadius, Math.PI / 2, startAngle,
    		RotateDirection.CCW, ToolCompensation.Out);
    	if(firstStep)
    	{
    		gcg.RapidMoveTo(start);
    		firstStep = false;
    	}
    	gcg.VerticalFeedTo(z);
    	gcg.HorizontalArc(center1, outRadius, Math.PI / 2, startAngle,
    		RotateDirection.CCW, ToolCompensation.Out);
    	if(bottomPart)
    	{	
    		gcg.HorizontalArc(center3, smallRadius, Math.PI, -0.9f,
    			RotateDirection.CCW, ToolCompensation.Out);
    		gcg.HorizontalArc(center2, outRadius, -Math.PI / 2 + 0.7f, Math.PI / 2,
    			RotateDirection.CCW, ToolCompensation.Out);
    	}
    	else
    	{
    		gcg.HorizontalArc(center2, outRadius, -Math.PI / 2, Math.PI / 2,
    			RotateDirection.CCW, ToolCompensation.Out);
    	}
    	

    	gcg.HorizontalFeedTo(start);
    }
    gcg.GoToSafetyHeight();
}

public static IEnumerable<IGCode> Generate()
{
    GCodeGenerator gcg = new GCodeGenerator();
    for(int x = 0; x < 7; x++)
    {
    	float xShift = x * 30;
    	for(int y = 0; y < 2; y++)
    	{
    		float yShift = - y * 44;
    		CreateHandler(gcg, new Vector2(xShift, yShift), false);
    		CreateHandler(gcg, new Vector2(xShift, yShift - 15 ), true);
    	}
    }
    return gcg.Codes;
}

