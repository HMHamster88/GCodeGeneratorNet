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
	var center = new Vector2();
	var sectorCount = 12;
	var sectorAngle = Math.PI * 2 / sectorCount;
	var sectorRadius = 140;
	var centralHoleRadius = 8;
	var shotButtonDistance = 110;
	var shotButtonHoleRadius = 3;
	
    GCodeGenerator gcg = new GCodeGenerator();
    
    foreach(float z in GCodeGenerator.range(gcg.MaterialHeight, 0, gcg.VerticalStep))
    {
    	var startAngle = (Angle)(- sectorAngle / 2);
    	var stopAngle = (Angle)( sectorAngle / 2);
    	var startPoint = center + startAngle.HorizontalVector * (centralHoleRadius - gcg.ToolRadiusAndTolerance);
    	var stopPoint = center + stopAngle.HorizontalVector * (centralHoleRadius - gcg.ToolRadiusAndTolerance);
    	var startD = (startAngle - Math.PI / 2).HorizontalVector * gcg.ToolRadiusAndTolerance;
    	var stopD = (stopAngle + Math.PI / 2).HorizontalVector * gcg.ToolRadiusAndTolerance;
    	gcg.HorizontalFeedTo(startPoint + startD);	
    	gcg.VerticalFeedTo(z);
    	gcg.HorizontalFeedTo(startPoint);
    	gcg.HorizontalArc(center, centralHoleRadius, startAngle, sectorAngle / 2, RotateDirection.CCW, ToolCompensation.In);
    	
    	
    	var startPoint2 = center + startAngle.HorizontalVector * (sectorRadius + gcg.ToolRadiusAndTolerance);
    	var stopPoint2 = center + stopAngle.HorizontalVector * (sectorRadius + gcg.ToolRadiusAndTolerance);
    	
    	if(z != 0)
    	{
    		gcg.HorizontalFeedTo(stopPoint + stopD);
    		gcg.HorizontalFeedTo(stopPoint2 + stopD);
    	}
    	else
    	{
    		gcg.DottedLine(stopPoint + stopD, stopPoint2 + stopD, z, 3, 0.5f, 3);
    	}
    	
    	gcg.HorizontalFeedTo(stopPoint2);
    	gcg.HorizontalFeedTo(startPoint2);
    	
    	if(z != 0)
    	{
    		gcg.HorizontalFeedTo(startPoint2 + startD);
    		gcg.HorizontalFeedTo(startPoint + startD);	
    	}
    	else
    	{
    		gcg.DottedLine(startPoint2 + startD, startPoint + startD, z, 3, 0.5f, 3);
    	}
    }
    
    gcg.RoundHole(new Vector2(shotButtonDistance, 0), shotButtonHoleRadius, gcg.MaterialHeight, 0);
    gcg.Pause();
    var m = new Matrix();
    m.Translate(shotButtonDistance, 0);
    m.Rotate(-90);
    gcg.Text("XII", FontFamily.GenericSansSerif, 0, 40, m, gcg.MaterialHeight - 1, 0.01f, true);
    return gcg.Codes;
}

