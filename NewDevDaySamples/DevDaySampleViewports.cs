using System;
using Rhino;
using Rhino.Commands;
using Rhino.UI;
using Eto.Forms;
using Eto.Drawing;

namespace NewDevDaySamples
{
    public class DevDaySampleViewports : Rhino.Commands.Command
    {
        public override string EnglishName => "DevDaySample4_ViewportControl";

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            var f = new SampleViewportForm();
            f.ShowModal(RhinoEtoApp.MainWindow);
            return Result.Success;
        }

        class SampleViewportForm : Dialog<Result>
        {
            public SampleViewportForm()
            {
                Title = "Rhino Viewport in an Eto Control";
                Resizable = true;
                var viewport_control = new[] { new Rhino.UI.Controls.ViewportControl {Size = new Size(400, 200)},
        new Rhino.UI.Controls.ViewportControl {Size = new Size(400, 200)} };
                Content = new StackLayout
                {
                    Padding = new Padding(2),
                    Spacing = 5,
                    HorizontalContentAlignment = HorizontalAlignment.Stretch,
                    Items =
        {
          viewport_control[0],
          viewport_control[1]
        }
                };
            }
        }
    }
}
