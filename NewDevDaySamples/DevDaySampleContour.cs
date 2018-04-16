using System;
using Rhino;
using Rhino.Commands;
using Rhino.Input;
using Rhino.Geometry;
using Rhino.DocObjects;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NewDevDaySamples
{
  class DevDaySampleContour : Command
  {
    public override string EnglishName => "DevDaySampleContour";

    protected override Result RunCommand(RhinoDoc doc, RunMode mode)
    {
      // mimic the functionality of the contour command using a background threads
      // The tricky parts to the contour command are that it dynamically adds
      // sections as they are found
      const ObjectType filter = ObjectType.Brep | ObjectType.Extrusion | ObjectType.Mesh;
      ObjRef[] objrefs;
      var rc = RhinoGet.GetMultipleObjects("Select objects for contours", false, filter, out objrefs);
      if (rc != Result.Success)
        return rc;
      if (objrefs == null || objrefs.Length < 1)
        return Result.Failure;

      Point3d base_point;
      rc = RhinoGet.GetPoint("Contour plane base point", false, out base_point);
      if (rc != Result.Success)
        return rc;
      var gp = new Rhino.Input.Custom.GetPoint();
      gp.SetCommandPrompt("Direction perpendicular to contour planes");
      gp.DrawLineFromPoint(base_point, false);
      gp.Get();
      if (gp.CommandResult() != Result.Success)
        return gp.CommandResult();
      Point3d end_point = gp.Point();

      double interval = 1;
      rc = RhinoGet.GetNumber("Distance between contours", true, ref interval, 0.001, 10000);
      if (rc != Result.Success)
        return rc;

      // figure out the combined bounding box of all the selected geometry
      BoundingBox bounds = BoundingBox.Unset;
      var geometries = new List<GeometryBase>();
      foreach (var objref in objrefs)
      {
        var geometry = objref.Geometry();
        var extrusion = geometry as Extrusion;
        if (extrusion != null)
          geometry = objref.Brep();
        geometries.Add(geometry);
        var bbox = geometry.GetBoundingBox(false);
        bounds.Union(bbox);
      }

      Vector3d normal = end_point - base_point;
      normal.Unitize();
      var curplane = new Plane(base_point, normal);
      double min_t, max_t;
      if (!curplane.DistanceTo(bounds, out min_t, out max_t))
        return Result.Failure;

      min_t -= interval;
      max_t += interval;
      min_t = Math.Floor(min_t / interval);
      max_t = Math.Ceiling(max_t / interval);
      double tolerance = doc.ModelAbsoluteTolerance;

      var tasks = new List<Task<Curve[]>>();
      var gc = new Rhino.Input.Custom.GetCancel();
      for (double t = min_t; t <= max_t; t += 1.0)
      {
        double offset = t * interval;
        var point = base_point + normal * offset;
        var plane = new Plane(point, normal);
        foreach (var geom in geometries)
        {
          var geom1 = geom;
          var task = Task.Run(() => Section(plane, geom1, tolerance), gc.Token);
          tasks.Add(task);
        }
      }
      gc.TaskCompleted += OnTaskCompleted;
      rc = gc.WaitAll(tasks, doc);
      return rc;
    }

    void OnTaskCompleted(object sender, Rhino.Input.Custom.TaskCompleteEventArgs e)
    {
      var t = e.Task as Task<Curve[]>;
      if (t != null && t.Status == TaskStatus.RanToCompletion)
      {
        var curves = t.Result;
        if (curves != null && curves.Length > 0)
        {
          foreach (var curve in curves)
            e.Doc.Objects.AddCurve(curve);
          e.Redraw = true;
        }
      }
    }

    static Curve[] Section(Plane plane, GeometryBase geometry, double tolerance)
    {
      var rc = new Curve[0];
      var brep = geometry as Brep;
      if (brep != null)
      {
        Point3d[] pts;
        Rhino.Geometry.Intersect.Intersection.BrepPlane(brep, plane, tolerance, out rc, out pts);
        return rc;
      }
      var mesh = geometry as Mesh;
      if (mesh != null)
      {
        var polylines = Rhino.Geometry.Intersect.Intersection.MeshPlane(mesh, plane);
        if (polylines != null)
        {
          rc = new Curve[polylines.Length];
          for (int i = 0; i < polylines.Length; i++)
          {
            rc[i] = new PolylineCurve(polylines[i]);
          }
        }
      }
      return rc;
    }
  }
}
