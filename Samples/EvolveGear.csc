using OpenTK;
using System;
using System.Collections;
using System.Collections.Generic;
using GCodeGeneratorNet.Core;
using GCodeGeneratorNet.Core.GCodes;
using GCodeGeneratorNet.Core.Misc;
using GCodeGeneratorNet.Core.Geometry;

public static Contour Gear(Vector2 center, int z, float m)
{
	PartBuilder pb = new PartBuilder();
	var has = 1.0f; 				// коэффициент высоты головки зуба
	var cs = 0.25f; 				// коэффициент радиального зазора
	var alpha = Angle.Degrees(20);	// угол зацепления
	var ha = has * m; 				// высота головок зуба
	var hf = (has + cs) * m; 		// глубина впадин
	var d = m*z;					// Делительный диаметр
	var da = d + ha * 2; 			// Диаметр вершин зубьев
	var df = d - hf * 2; 			// Диаметр впадин
	var db = Math.Cos(alpha) * d;	// Диаметр основной окружности
	var ra = da / 2;
	var rb = db / 2;
	
	var oa = ra;
	var ob = rb;
	var ab = oa - ob;
	var ac = ab / 3;
	var oc = oa - ac;
	var od = rb;
	var dc = Math.Sqrt(oc * oc - od * od);
	var ec = dc * 0.75f;
	var eb = (od * ec) / oc;
	var oe = Math.Sqrt(ob * ob + eb * eb);
	var beo = Math.Asin(ob / oe);
	var a1e = ec;
	var a1eo = Math.Acos((a1e * a1e + oe * oe - oa * oa)/ (2 * a1e * oe));
	var a1eb = a1eo - beo;
	GDebug.WriteLine("ec = " + ec);
	GDebug.WriteLine("eb = " + eb);
	GDebug.WriteLine("ob = " + ob);
	GDebug.WriteLine("od = " + od);
	GDebug.WriteLine("oe = " + oe);
	
	GDebug.WriteLine("beo = " + 180 * beo / Math.PI );
	GDebug.WriteLine("a1eo = " + 180 * a1eo / Math.PI );
	GDebug.WriteLine("a1eb = " + 180 * a1eb / Math.PI );
	
	foreach(Angle angle in Angle.Angles(z))
	{
		var ecoords = angle.HorizontalVector * (float)ob + 
		(angle + Math.PI / 2).HorizontalVector * (float)eb;
		pb.AddArc(ecoords, (float)ec, angle - Math.PI / 2, angle - a1eb, RotateDirection.CCW);
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
    pb.SetContour(Gear(new Vector2(), 20, 3));
    
    /*foreach(Angle angle in Angle.Angles(6))
	{
		pb.AddCircle(angle.HorizontalVector * 55, 25);
		pb.CreateHole();		
	}*/

    parts.Add(pb.CreatePart(4));


    foreach(var part in parts)
    {
    	gcg.Part25D(part);
    }
    gcg.GoToSafetyHeight();
    return new GScriptResult(parts, gcg.Codes);
}

