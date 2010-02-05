/*
//-------------------------------------------------------------------------------//
This file is part of tracm, a client for the ACM Shared Content Server.
Copyright (C) 2010 Tightrope Media Systems
http://www.trms.com
http://labs.trms.com

tracm is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License.

tracm is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.
//-------------------------------------------------------------------------------//
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using tracm.Properties;
using tracm.Queue;

namespace tracm
{
	public class DeleteWorker : WorkItem
	{
		private string m_path;

		public DeleteWorker(string path)
		{
			m_path = path;
			Progress.ShowProgressBar = false;
		}

		protected override void WorkMethod()
		{
			if (IsRunning && File.Exists(m_path))
				File.Delete(m_path);
		}

		public override string ToString()
		{
			return String.Format("Cleaning up {0}", Path.GetFileName(m_path));
		}
	}
}
