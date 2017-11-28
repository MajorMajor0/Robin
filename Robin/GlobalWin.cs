﻿using BizHawk.Bizware.BizwareGL;

// ReSharper disable StyleCop.SA1401
namespace Robin
{
	public static class GlobalWin
	{
		public static MainWindow MainForm;
		public static ToolManager Tools;

		/// <summary>
		/// the IGL to be used for rendering
		/// </summary>
		public static IGL GL;

		/// <summary>
		/// The IGL_TK to be used for specifically opengl operations (accessing textures from opengl-based cores)
		/// </summary>
		public static Bizware.BizwareGL.Drivers.OpenTK.IGL_TK IGL_GL;

		public static Sound Sound;
		public static readonly OSDManager OSD = new OSDManager();
		public static DisplayManager DisplayManager;
		public static GLManager GLManager;

		public static int ExitCode;
	}
}
