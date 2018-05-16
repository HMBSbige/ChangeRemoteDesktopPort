using System;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Windows.Forms;
using System.Security.Principal;

namespace ChangeRemoteDesktopPort
{
	public static class AdminButton
	{
		[DllImport(@"user32")]
		private static extern uint SendMessage(IntPtr hWnd, uint msg, uint wParam, uint lParam);

		private const int BCM_FIRST = 0x1600;
		private const int BCM_SETSHIELD = BCM_FIRST + 0x000C;

		internal static bool IsVistaOrHigher()
		{
			return Environment.OSVersion.Version.Major < 6;
		}

		/// <summary>
		/// Checks if the process is elevated
		/// </summary>
		/// <returns>If is elevated</returns>
		internal static bool IsAdmin()
		{
			var id = WindowsIdentity.GetCurrent();
			var p = new WindowsPrincipal(id);
			return p.IsInRole(WindowsBuiltInRole.Administrator);
		}

		/// <summary>
		/// Add a shield icon to a button
		/// </summary>
		/// <param name="b">The button</param>
		internal static void AddShieldToButton(Button b)
		{
			b.FlatStyle = FlatStyle.System;
			SendMessage(b.Handle, BCM_SETSHIELD, 0, 0xFFFFFFFF);
		}

		/// <summary>
		/// Restart the current process with administrator credentials
		/// </summary>
		internal static void RestartElevated()
		{
			var startInfo = new ProcessStartInfo
			{
					UseShellExecute = true,
					WorkingDirectory = Environment.CurrentDirectory,
					FileName = Application.ExecutablePath,
					Verb = @"runas"
			};
			try
			{
				Process.Start(startInfo);
			}
			catch (System.ComponentModel.Win32Exception)
			{
				return; //If cancelled, do nothing
			}

			Application.Exit();
		}
	}
}