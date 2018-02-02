using System;
using System.Windows.Forms;
using ChangeRemoteDesktopPort.Properties;
using Microsoft.Win32;

namespace ChangeRemoteDesktopPort
{
    public partial class ChangeRemoteDesktopPort : Form
    {
        //HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Terminal Server\WinStations\RDP-Tcp\PortNumber

        public ChangeRemoteDesktopPort()
        {
            InitializeComponent();
        }

        private void ChangeRemoteDesktopPort_Load(object sender, EventArgs e)
        {
            var portnumber = GetPort();
            if (portnumber == 0)
            {
                MessageBox.Show(@"尝试获取端口失败!", @"错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(1);
            }
            else
            {
                Icon = Resources.all;
                port.Value = portnumber;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var portnumber = port.Value;
            try
            {
                ChangePort(Convert.ToInt32(portnumber));
            }
            catch
            {
                MessageBox.Show(@"尝试修改端口失败!可能没有管理员权限", @"错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (GetPort() != portnumber)
            {
                MessageBox.Show(@"端口修改失败，建议手动修改", @"错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                port.Value = portnumber;
                MessageBox.Show(@"端口修改成功！别忘记修改防火墙规则", @"成功",MessageBoxButtons.OK,MessageBoxIcon.Information);
            }
        }

        private static int GetPort()
        {
            var PortPath = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\Terminal Server\WinStations\RDP-Tcp", false);
            return Convert.ToInt32(PortPath.GetValue(@"PortNumber", 0).ToString());
        }

        private static void ChangePort(int portnumber)
        {
            //需要管理员权限
            var PortPath = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\Terminal Server\WinStations\RDP-Tcp", true);
            PortPath.SetValue(@"PortNumber", portnumber, RegistryValueKind.DWord);
        }
    }
}