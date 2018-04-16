using System;
using Rhino;
using Rhino.Commands;
using Rhino.Display;
using Rhino.Geometry;
using Rhino.DocObjects;

namespace NewDevDaySamples
{
  public class DevDaySampleSymbols : Command
  {
    ArrowConduit m_conduit = new ArrowConduit();

    public override string EnglishName => "DevDaySampleSymbols";

    protected override Result RunCommand(RhinoDoc doc, RunMode mode)
    {
      m_conduit.Enabled = !m_conduit.Enabled;
      doc.Views.Redraw();
      return Result.Success;
    }

    class ArrowConduit : DisplayConduit
    {
      protected override void PostDrawObjects(DrawEventArgs e)
      {
        //e.Display.DrawPoint(Point3d.Origin, PointStyle.Arrow, System.Drawing.Color.Black, System.Drawing.Color.White, 16, 1, 0, 0, true, true);
        var points = new[] { new Point3d(2, 2, 0), new Point3d(2, -6, 0), new Point3d(5, -6, 0), new Point3d(5, 2, 0) };
        e.Display.DrawPolygon(points, System.Drawing.Color.Black, false);

        System.Drawing.Color fill = System.Drawing.Color.FromArgb(250, System.Drawing.Color.White);

        Point3d center = new Point3d(2, 2, 0);
        Point3d off_x = new Point3d(3, 2, 0);
        Point3d off_y = new Point3d(2, 3, 0);
        float radius = 3;
        e.Display.DrawPoint(center, PointStyle.RoundControlPoint, System.Drawing.Color.Black, fill, radius, 1, 0, 0, true, true);
        Transform w2s = e.Viewport.GetTransform(CoordinateSystem.World, CoordinateSystem.Screen);
        Point3d center_screen = center;
        center_screen.Transform(w2s);
        Point3d off_screen = off_x;
        off_screen.Transform(w2s);
        Vector3d xaxis = new Vector3d(off_screen.X - center_screen.X, off_screen.Y - center_screen.Y, 0);
        double angle = Vector3d.VectorAngle(Vector3d.XAxis, xaxis);
        if (xaxis.Y > 0)
          angle = -angle;
        e.Display.DrawPoint(center, PointStyle.ArrowTail, System.Drawing.Color.Black, fill, 6, 1, 3, (float)angle, true, true);
        e.Display.DrawPoint(center, PointStyle.ArrowTail, System.Drawing.Color.Black, fill, 6, 1, 3, (float)(angle + RhinoMath.ToRadians(180)), true, true);

        off_screen = off_y;
        off_screen.Transform(w2s);
        Vector3d yaxis = new Vector3d(off_screen.X - center_screen.X, off_screen.Y - center_screen.Y, 0);
        angle = Vector3d.VectorAngle(Vector3d.YAxis, yaxis);
        if (yaxis.X < 0)
          angle = -angle;
        e.Display.DrawPoint(center, PointStyle.ArrowTail, System.Drawing.Color.Black, fill, 6, 1, 3, (float)(angle + RhinoMath.ToRadians(90)), true, true);
        e.Display.DrawPoint(center, PointStyle.ArrowTail, System.Drawing.Color.Black, fill, 6, 1, 3, (float)(angle + RhinoMath.ToRadians(270)), true, true);


        center = new Point3d(5, -2, 0);
        e.Display.DrawPoint(center, PointStyle.RoundControlPoint, System.Drawing.Color.Black, fill, radius, 1, 0, 0, true, true);
        center_screen = center;
        center_screen.Transform(w2s);
        off_screen = center;
        off_screen.X += 1;
        off_screen.Transform(w2s);
        xaxis = new Vector3d(off_screen.X - center_screen.X, off_screen.Y - center_screen.Y, 0);
        angle = Vector3d.VectorAngle(Vector3d.XAxis, xaxis);
        if (xaxis.Y > 0)
          angle = -angle;
        e.Display.DrawPoint(center, PointStyle.ArrowTail, System.Drawing.Color.Black, fill, 6, 1, 3, (float)angle, true, true);
        e.Display.DrawPoint(center, PointStyle.ArrowTail, System.Drawing.Color.Black, fill, 6, 1, 3, (float)(angle + RhinoMath.ToRadians(180)), true, true);

        e.Display.DrawPoint(points[1], PointStyle.Pin, System.Drawing.Color.Black, System.Drawing.Color.PeachPuff, 10, 1, 3, 0, true, true);

        base.PostDrawObjects(e);
      }
    }
  }
}
