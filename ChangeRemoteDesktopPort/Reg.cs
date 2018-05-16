using System;
using Microsoft.Win32;

namespace Common
{
	internal static class Reg
	{
		private static bool CheckPath(ref string path, out RegistryKey rk)
		{
			if (path.Contains(@"/"))
			{
				path = path.Replace('/', '\\');
			}

			if (path.StartsWith(@"计算机\") || path.StartsWith(@"Computer\"))
			{
				path = path.Substring(path.IndexOf('\\') + 1);
			}

			if (path.StartsWith(@"HKEY_LOCAL_MACHINE\"))
			{
				rk = Registry.LocalMachine;
				path = path.Substring(path.IndexOf('\\') + 1);
			}
			else if (path.StartsWith(@"HKEY_CLASSES_ROOT\"))
			{
				rk = Registry.ClassesRoot;
				path = path.Substring(path.IndexOf('\\') + 1);
			}
			else if (path.StartsWith(@"HKEY_CURRENT_CONFIG\"))
			{
				rk = Registry.CurrentConfig;
				path = path.Substring(path.IndexOf('\\') + 1);
			}
			else if (path.StartsWith(@"HKEY_CURRENT_USER\"))
			{
				rk = Registry.CurrentUser;
				path = path.Substring(path.IndexOf('\\') + 1);
			}
			else if (path.StartsWith(@"HKEY_PERFORMANCE_DATA\"))
			{
				rk = Registry.PerformanceData;
				path = path.Substring(path.IndexOf('\\') + 1);
			}
			else if (path.StartsWith(@"HKEY_USERS\"))
			{
				rk = Registry.Users;
				path = path.Substring(path.IndexOf('\\') + 1);
			}
			else
			{
				rk = null;
				return false;
			}
			return true;
		}

		/// <summary>
		/// 读取注册表的值
		/// 若不存在，则会返回 null
		/// </summary>
		/// <param name="path">路径</param>
		/// <param name="name">键名称</param>
		/// <returns></returns>
		public static object Read(string path, string name)
		{
			if (!CheckPath(ref path, out var root))
			{
				throw new ArgumentException(@"注册表路径格式错误！", nameof(path));
			}
			var fullpath = root.OpenSubKey(path, false);
			if (fullpath == null)
			{
				//throw new ArgumentException(@"指定路径不存在", nameof(path));
				return null;
			}

			return fullpath.GetValue(name);
		}

		/// <summary>
		/// 设置注册表的值
		/// 若不存在，则会自动创建
		/// </summary>
		/// <param name="path">路径</param>
		/// <param name="name">键名称</param>
		/// <param name="value">值</param>
		/// <param name="typeofvalue">值的数据类型</param>
		public static void Set(string path, string name, object value, RegistryValueKind typeofvalue)
		{
			if (!CheckPath(ref path, out var root))
			{
				throw new ArgumentException(@"注册表路径格式错误！", nameof(path));
			}
			var fullpath = root.CreateSubKey(path, true);

			fullpath.SetValue(name, value, typeofvalue);
			fullpath.Close();
		}

		/// <summary>
		/// 删除注册表的键
		/// 若不存在则忽略
		/// </summary>
		/// <param name="path">路径</param>
		/// <param name="name">键名称</param>
		public static void Delete(string path, string name)
		{
			if (!CheckPath(ref path, out var root))
			{
				throw new ArgumentException(@"注册表路径格式错误！", nameof(path));
			}

			var fullpath = root.OpenSubKey(path, true);
			if (fullpath == null)
			{
				return;
			}

			fullpath.DeleteValue(name);
			fullpath.Close();
		}

		/// <summary>
		/// 删除注册表的项
		/// 若不存在则忽略
		/// </summary>
		/// <param name="path">路径</param>
		public static void Delete(string path)
		{
			if (!CheckPath(ref path, out var root))
			{
				throw new ArgumentException(@"注册表路径格式错误！", nameof(path));
			}

			root.DeleteSubKey(path,false);
			root.Close();
		}

		/// <summary>
		/// 创建注册表的项
		/// 若已存在则忽略
		/// </summary>
		/// <param name="path"></param>
		public static void CreateSubKey(string path)
		{
			if (!CheckPath(ref path, out var root))
			{
				throw new ArgumentException(@"注册表路径格式错误！", nameof(path));
			}

			root.CreateSubKey(path,false);
			root.Close();
		}
	}
}
