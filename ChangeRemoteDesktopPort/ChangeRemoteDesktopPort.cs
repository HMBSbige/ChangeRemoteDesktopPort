using System;
using System.Windows.Forms;
using ChangeRemoteDesktopPort.Properties;
using Common;
using Microsoft.Win32;

namespace ChangeRemoteDesktopPort
{
	public partial class ChangeRemoteDesktopPort : Form
	{
		private const string RegPath = @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Terminal Server\WinStations\RDP-Tcp\";
		private const string RegName = @"PortNumber";

		public ChangeRemoteDesktopPort()
		{
			InitializeComponent();
			if (!AdminButton.IsAdmin())
			{
				AdminButton.AddShieldToButton(button1);
			}
		}

		private void ChangeRemoteDesktopPort_Load(object sender, EventArgs e)
		{
			var portnumber = GetPort();
			if (portnumber == 0)
			{
				portnumber = 3389;
				MessageBox.Show(@"获取当前端口失败", @"错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
			Icon = Resources.all;
			port.Value = portnumber;
		}

		private void button1_Click(object sender, EventArgs e)
		{
			if (AdminButton.IsAdmin())
			{
				ChangePort();
			}
			else
			{
				AdminButton.RestartElevated();
			}
		}

		private void ChangePort()
		{
			try
			{
				var portnumber = port.Value;
				SetPort(Convert.ToInt32(portnumber));
				if (GetPort() != portnumber)
				{
					MessageBox.Show(@"端口修改失败，建议手动修改", @"错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
				else
				{
					port.Value = portnumber;
					MessageBox.Show(@"端口修改成功！别忘记修改防火墙规则", @"成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
				}
			}
			catch (UnauthorizedAccessException)
			{
				MessageBox.Show(@"尝试修改端口失败!没有管理员权限", @"错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, ex.Data.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		private static int GetPort()
		{
			var res = Reg.Read(RegPath, RegName);
			return res == null ? 0 : Convert.ToInt32(Reg.Read(RegPath, RegName));
		}

		private static void SetPort(int portnumber)
		{
			//需要管理员权限
			Reg.Set(RegPath, RegName, portnumber, RegistryValueKind.DWord);
		}
	}
}