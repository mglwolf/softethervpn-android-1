﻿// CoreUtil


using System;
using System.Threading;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Text;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Diagnostics;
using System.Web.Mail;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Net.Mail;
using System.Net.Mime;
using System.Reflection;
using CoreUtil;

namespace CoreUtil
{
	public static class Env
	{
		static object lockObj = new object();
		static bool inited = false;

		static Env()
		{
			initCache();
		}

		static void initCache()
		{
			lock (lockObj)
			{
				if (inited == false)
				{
					initValues();
					inited = true;
				}
			}
		}

		static string homeDir;
		static public string HomeDir
		{
			get { return homeDir; }
		}
		static string exeFileName;
		static public string ExeFileName
		{
			get { return exeFileName; }
		}
		static string exeFileDir;
		static public string ExeFileDir
		{
			get { return exeFileDir; }
		}
		static string windowsDir;
		static public string WindowsDir
		{
			get { return windowsDir; }
		}
		static string systemDir;
		static public string SystemDir
		{
			get { return systemDir; }
		}
		static string tempDir;
		static public string TempDir
		{
			get { return tempDir; }
		}
		static string winTempDir;
		static public string WinTempDir
		{
			get { return winTempDir; }
		}
		static string windowsDrive;
		static public string WindowsDrive
		{
			get { return windowsDrive; }
		}
		static string programFilesDir;
		static public string ProgramFilesDir
		{
			get { return programFilesDir; }
		}
		static string personalStartMenuDir;
		static public string PersonalStartMenuDir
		{
			get { return personalStartMenuDir; }
		}
		static string personalProgramsDir;
		static public string PersonalProgramsDir
		{
			get { return personalProgramsDir; }
		}
		static string personalStartupDir;
		static public string PersonalStartupDir
		{
			get { return personalStartupDir; }
		}
		static string personalAppDataDir;
		static public string PersonalAppDataDir
		{
			get { return personalAppDataDir; }
		}
		static string personalDesktopDir;
		static public string PersonalDesktopDir
		{
			get { return personalDesktopDir; }
		}
		static string myDocumentsDir;
		static public string MyDocumentsDir
		{
			get { return myDocumentsDir; }
		}
		static string localAppDataDir;
		static public string LocalAppDataDir
		{
			get { return localAppDataDir; }
		}
		static string userName;
		static public string UserName
		{
			get { return userName; }
		}
		static string userNameEx;
		static public string UserNameEx
		{
			get { return userNameEx; }
		}
		static string machineName;
		static public string MachineName
		{
			get { return machineName; }
		}
		static string commandLine;
		public static string CommandLine
		{
			get { return commandLine; }
		}
		public static StrToken CommandLineList
		{
			get
			{
				return new StrToken(CommandLine);
			}
		}
		static OperatingSystem osInfo;
		public static OperatingSystem OsInfo
		{
			get { return osInfo; }
		}
		static bool isNt;
		public static bool IsNt
		{
			get { return isNt; }
		}
		static bool is9x;
		public static bool Is9x
		{
			get { return is9x; }
		}
		static bool isCe;
		public static bool IsCe
		{
			get { return isCe; }
		}
		static bool isLittleEndian;
		public static bool IsLittleEndian
		{
			get { return Env.isLittleEndian; }
		}
		public static bool IsBigEndian
		{
			get { return !IsLittleEndian; }
		}
		static bool isAdmin;
		public static bool IsAdmin
		{
			get { return Env.isAdmin; }
		}
		static int processId;
		public static int ProcessId
		{
			get { return Env.processId; }
		}
		static string myTempDir;
		public static string MyTempDir
		{
			get { return myTempDir; }
		}
		static IO lockFile;

		public static bool Is64BitProcess
		{
			get
			{
				return (IntPtr.Size == 8);
			}
		}

		public static bool Is64BitWindows
		{
			get
			{
				return Is64BitProcess || Kernel.InternalCheckIsWow64();
			}
		}

		public static bool IsWow64
		{
			get
			{
				return Kernel.InternalCheckIsWow64();
			}
		}

		static void initValues()
		{
			exeFileName = IO.RemoteLastEnMark(getMyExeFileName());
			if (Str.IsEmptyStr(exeFileName) == false)
			{
				exeFileDir = IO.RemoteLastEnMark(Path.GetDirectoryName(exeFileName));
			}
			else
			{
				exeFileDir = "";
			}
			homeDir = IO.RemoteLastEnMark(Kernel.GetEnvStr("HOME"));
			if (Str.IsEmptyStr(homeDir))
			{
				homeDir = IO.RemoteLastEnMark(Kernel.GetEnvStr("HOMEDRIVE") + Kernel.GetEnvStr("HOMEPATH"));
			}
			if (Str.IsEmptyStr(homeDir))
			{
				homeDir = CurrentDir;
			}
			systemDir = IO.RemoteLastEnMark(Environment.GetFolderPath(Environment.SpecialFolder.System));
			windowsDir = IO.RemoteLastEnMark(Path.GetDirectoryName(systemDir));
			tempDir = IO.RemoteLastEnMark(Path.GetTempPath());
			winTempDir = IO.RemoteLastEnMark(Path.Combine(windowsDir, "Temp"));
			IO.MakeDir(winTempDir);
			if (windowsDir.Length >= 2 && windowsDir[1] == ':')
			{
				windowsDrive = windowsDir.Substring(0, 2).ToUpper();
			}
			else
			{
				windowsDrive = "C:";
			}
			programFilesDir = IO.RemoteLastEnMark(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles));
			personalStartMenuDir = IO.RemoteLastEnMark(Environment.GetFolderPath(Environment.SpecialFolder.StartMenu));
			personalProgramsDir = IO.RemoteLastEnMark(Environment.GetFolderPath(Environment.SpecialFolder.Programs));
			personalStartupDir = IO.RemoteLastEnMark(Environment.GetFolderPath(Environment.SpecialFolder.Startup));
			personalAppDataDir = IO.RemoteLastEnMark(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
			personalDesktopDir = IO.RemoteLastEnMark(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory));
			myDocumentsDir = IO.RemoteLastEnMark(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
			localAppDataDir = IO.RemoteLastEnMark(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
			userName = Environment.UserName;
			try
			{
				userNameEx = Environment.UserDomainName + "\\" + userName;
			}
			catch
			{
				userNameEx = userName;
			}
			machineName = Environment.MachineName;
			commandLine = initCommandLine(Environment.CommandLine);
			osInfo = Environment.OSVersion;
			isNt = (osInfo.Platform == PlatformID.Win32NT);
			isCe = (osInfo.Platform == PlatformID.WinCE);
			is9x = !(isNt || isCe);
			isLittleEndian = BitConverter.IsLittleEndian;
			processId = System.Diagnostics.Process.GetCurrentProcess().Id;
			isAdmin = checkIsAdmin();
			initMyTempDir();
		}

		static void deleteUnusedTempDir()
		{
			DirEntry[] files;

			files = IO.EnumDir(Env.tempDir);

			foreach (DirEntry e in files)
			{
				if (e.IsFolder)
				{
					if (e.FileName.StartsWith("NET_", StringComparison.CurrentCultureIgnoreCase) && e.FileName.Length == 8)
					{
						string dirFullName = Path.Combine(Env.tempDir, e.fileName);
						string lockFileName = Path.Combine(dirFullName, "LockFile.dat");
						bool deleteNow = false;

						try
						{
							IO io = IO.FileOpen(lockFileName);
							io.Close();

							try
							{
								io = IO.FileOpen(lockFileName, true);
								deleteNow = true;
								io.Close();
							}
							catch
							{
							}
						}
						catch
						{
							DirEntry[] files2;

							deleteNow = true;

							try
							{
								files2 = IO.EnumDir(dirFullName);

								foreach (DirEntry e2 in files2)
								{
									if (e2.IsFolder == false)
									{
										string fullPath = Path.Combine(dirFullName, e2.fileName);

										try
										{
											IO io2 = IO.FileOpen(fullPath, true);
											io2.Close();
										}
										catch
										{
											deleteNow = false;
										}
									}
								}
							}
							catch
							{
								deleteNow = false;
							}
						}

						if (deleteNow)
						{
							IO.DeleteDir(dirFullName, true);
						}
					}
				}
			}
		}

		static void initMyTempDir()
		{
			try
			{
				deleteUnusedTempDir();
			}
			catch
			{
			}

			int num = 0;

			while (true)
			{
				byte[] rand = Secure.Rand(2);
				string tmp2 = Str.ByteToStr(rand);

				string tmp = Path.Combine(Env.tempDir, "NET_" + tmp2);

				if (IO.IsDirExists(tmp) == false && IO.MakeDir(tmp))
				{
					Env.myTempDir = tmp;

					break;
				}

				if ((num++) >= 100)
				{
					throw new SystemException();
				}
			}

			string lockFileName = Path.Combine(Env.myTempDir, "LockFile.dat");
			lockFile = IO.FileCreate(lockFileName);
		}

		static bool checkIsAdmin()
		{
			try
			{
				string name = "Vpn_Check_Admin_Key_NET_" + processId.ToString();
				string teststr = Str.GenRandStr();

				if (Reg.WriteStr(RegRoot.LocalMachine, "", name, teststr) == false)
				{
					return false;
				}

				try
				{

					string ret = Reg.ReadStr(RegRoot.LocalMachine, "", name);

					if (ret == teststr)
					{
						return true;
					}

					return false;
				}
				finally
				{
					Reg.DeleteValue(RegRoot.LocalMachine, "", name);
				}
			}
			catch
			{
				return false;
			}
		}

		static string initCommandLine(string src)
		{
			try
			{
				int i;
				if (src.Length >= 1 && src[0] == '\"')
				{
					i = src.IndexOf('\"', 1);
				}
				else
				{
					i = src.IndexOf(' ');
				}

				if (i == -1)
				{
					return "";
				}
				else
				{
					return src.Substring(i + 1).TrimStart(' ');
				}
			}
			catch
			{
				return "";
			}
		}

		static string getMyExeFileName()
		{
			try
			{
				Assembly mainAssembly = Assembly.GetEntryAssembly();
				Module[] modules = mainAssembly.GetModules();
				return modules[0].FullyQualifiedName;
			}
			catch
			{
				return "";
			}
		}

		static public string CurrentDir
		{
			get
			{
				return IO.RemoteLastEnMark(Environment.CurrentDirectory);
			}
		}
		static public string NewLine
		{
			get
			{
				return Environment.NewLine;
			}
		}
	}
}
