//
// MSBuildTargetTimingsPad.cs
//
// Author:
//       Matt Ward <matt.ward@microsoft.com>
//
// Copyright (c) 2019 Microsoft
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Threading.Tasks;
using Gtk;
using MonoDevelop.Components;
using MonoDevelop.Components.Docking;
using MonoDevelop.Core;
using MonoDevelop.Ide.Gui;
using MonoDevelop.Ide.Gui.Components;
using MonoDevelop.Ide;
using MonoDevelop.Projects;

namespace MonoDevelop.MSBuildTargetTimings
{
	class MSBuildTargetTimingsPad : PadContent
	{
		MSBuildTargetTimingWidget widget;
		Control control;
		Button runTestsButton;

		public override Control Control {
			get {
				if (control == null) {
					widget = new MSBuildTargetTimingWidget ();
					control = new XwtControl (widget);
				}
				return control;
			}
		}

		protected override void Initialize (IPadWindow window)
		{
			var toolbar = window.GetToolbar (DockPositionType.Right);

			runTestsButton = new Button (new ImageView (Ide.Gui.Stock.RunProgramIcon, IconSize.Menu));
			runTestsButton.Clicked += OnRunTestsButtonClick;
			runTestsButton.TooltipText = GettextCatalog.GetString ("Run Tests");
			toolbar.Add (runTestsButton);

			toolbar.ShowAll ();
		}

		void OnRunTestsButtonClick (object sender, EventArgs e)
		{
			runTestsButton.Sensitive = false;
			RunTests ().ContinueWith (t => {
				if (t.Exception != null)
					LoggingService.LogError ("RunTests error", t.Exception);

				Runtime.RunInMainThread (() => {
					if (t.Exception != null)
						LogError ("Error: {0}", t.Exception);
					runTestsButton.Sensitive = true;
				});
			});
		}

		void LogError (string format, params object[] args)
		{
			string message = string.Format (format + Environment.NewLine, args);
			LogView.WriteError (message);
		}

		void Log (string format, params object [] args)
		{
			string message = string.Format (format + Environment.NewLine, args);
			LogView.WriteText (message);
		}

		public LogView LogView {
			get { return widget.LogView; }
		}

		async Task RunTests ()
		{
			var project = IdeApp.ProjectOperations.CurrentSelectedProject as DotNetProject;
			if (project == null) {
				Log ("No project selected.");
				return;
			}

			await project.ClearCachedData ();

			var config = IdeApp.Workspace.ActiveConfiguration ?? ConfigurationSelector.Default;

			using (var timer = new SimpleTimer (LogView, "GetReferencedAssemblies")) {
				var results = await project.GetReferencedAssemblies (config);
			}

			using (var timer = new SimpleTimer (LogView, "GetSourceFilesAsync")) {
				var sources = await project.GetSourceFilesAsync (config);
			}
		}
	}
}
