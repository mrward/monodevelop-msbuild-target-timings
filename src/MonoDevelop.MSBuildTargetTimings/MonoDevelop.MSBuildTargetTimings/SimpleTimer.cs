//
// SimpleTimer.cs
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
using System.Diagnostics;
using MonoDevelop.Ide.Gui.Components;
using MonoDevelop.Projects;

namespace MonoDevelop.MSBuildTargetTimings
{
	class SimpleTimer : IDisposable
	{
		readonly LogView logView;
		readonly string timing;
		readonly Stopwatch stopwatch;
		readonly string projectName;

		public SimpleTimer (LogView logView, Project project, string timing)
		{
			this.logView = logView;
			this.timing = timing;
			this.projectName = project.Name;

			stopwatch = new Stopwatch ();
			stopwatch.Start ();
		}

		public void Dispose ()
		{
			stopwatch.Stop ();

			string message = string.Format ("[{0}] {1} {2} ms{3}", projectName, timing, stopwatch.ElapsedMilliseconds, Environment.NewLine);

			logView.WriteText (message);
		}
	}
}
