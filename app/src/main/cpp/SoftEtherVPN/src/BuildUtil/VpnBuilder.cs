// SoftEther VPN Source Code - Developer Edition Master Branch
// Build Utility


using System;
using System.Threading;
using System.Text;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
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
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using CoreUtil;

namespace BuildUtil
{
	// Languages
	public class Language
	{
		public int Number;
		public string Id;
		public string Title;
		public string TitleUnicode;
		public string WindowsLocaleIds;
		public string UnixLocaleIds;
	}

	// Build helper class
	public static class BuildHelper
	{
		// loads the language list text file
		public static Language[] GetLanguageList()
		{
			return GetLanguageList(Path.Combine(Paths.BinDirName, @"hamcore\languages.txt"));
		}
		public static Language[] GetLanguageList(string filename)
		{
			List<Language> ret = new List<Language>();
			string[] lines = File.ReadAllLines(filename, Str.Utf8Encoding);

			foreach (string line in lines)
			{
				string s = line.Trim();

				if (Str.IsEmptyStr(s) == false)
				{
					if (s.StartsWith("#", StringComparison.InvariantCultureIgnoreCase) == false)
					{
						string[] sps = { " ", "\t", };
						string[] tokens = s.Split(sps, StringSplitOptions.RemoveEmptyEntries);

						if (tokens.Length == 6)
						{
							Language e = new Language();

							e.Number = Str.StrToInt(tokens[0]);
							e.Id = tokens[1];
							e.Title = Str.ReplaceStr(tokens[2], "_", " ");
							e.TitleUnicode = tokens[3];
							e.WindowsLocaleIds = tokens[4];
							e.UnixLocaleIds = tokens[5];

							ret.Add(e);

							Con.WriteLine(tokens.Length);
						}
					}
				}
			}

			return ret.ToArray();
		}

		// Build
		public static void BuildMain(BuildSoftware soft, bool debugModeIfUnix)
		{
			int versionMajor, versionMinor, versionBuild;
			string name;
			DateTime date;

			string title = Console.Title;
			Console.Title = string.Format("Building {0}", soft.IDString);

			try
			{
				Win32BuildUtil.ReadBuildInfoFromTextFile(out versionMajor, out versionMinor, out versionBuild, out name, out date);

				soft.SetBuildNumberVersionName(versionMajor, versionMinor, versionBuild, name, date);

				Con.WriteLine("Building '{0}' - {1}...", soft.IDString, soft.TitleString);

				BuildSoftwareUnix softUnix = soft as BuildSoftwareUnix;

				if (softUnix == null)
				{
					soft.Build();
				}
				else
				{
					softUnix.Build(debugModeIfUnix);
				}
			}
			finally
			{
				Console.Title = title;
			}
		}

		// Convert the number to a version number
		public static string VersionIntToString(int versionMajor, int versionMinor)
		{
			return string.Format("{0}.{1:D2}", versionMajor, versionMinor);
		}

		// Get a product list that is included in the software
		public static string GetSoftwareProductList(Software soft)
		{
			string ret = "";

			switch (soft)
			{
				case Software.vpnbridge:
					ret = "PacketiX VPN Bridge";
					break;

				case Software.vpnclient:
					ret = "PacketiX VPN Client, PacketiX VPN Command-Line Admin Utility (vpncmd)";
					break;

				case Software.vpnserver:
					ret = "PacketiX VPN Server, PacketiX VPN Command-Line Admin Utility (vpncmd)";
					break;

				case Software.vpnserver_vpnbridge:
					ret = "PacketiX VPN Server, PacketiX VPN Bridge, PacketiX VPN Server Manager for Windows, PacketiX VPN Command-Line Admin Utility (vpncmd)";
					break;

				default:
					throw new ApplicationException("invalid soft.");
			}

#if BU_SOFTETHER
			ret = Str.ReplaceStr(ret, "PacketiX", "SoftEther", false);
#endif

			return ret;
		}

		// Get the title of the software
		public static string GetSoftwareTitle(Software soft)
		{
			string ret = "";

			switch (soft)
			{
				case Software.vpnbridge:
					ret = "PacketiX VPN Bridge";
					break;

				case Software.vpnclient:
					ret = "PacketiX VPN Client";
					break;

				case Software.vpnserver:
					ret = "PacketiX VPN Server";
					break;

				case Software.vpnserver_vpnbridge:
					ret = "PacketiX VPN Server and VPN Bridge";
					break;

				default:
					throw new ApplicationException("invalid soft.");
			}
			
#if BU_SOFTETHER
			ret = Str.ReplaceStr(ret, "PacketiX", "SoftEther", false);
#endif

			return ret;
		}
	}

	// Basic path information
	public static class Paths
	{
		public static readonly string ExeFileName = Env.ExeFileName;
		public static readonly string ExeDirName = Env.ExeFileDir;
		public static readonly string BinDirName = ExeDirName;
		public static readonly string BaseDirName = IO.NormalizePath(Path.Combine(BinDirName, @"..\"));
		public static readonly string UtilityDirName = IO.NormalizePath(Path.Combine(BinDirName, @"..\BuildFiles\Utility"));

#if !BU_SOFTETHER
		// PacketiX VPN (build by SoftEther)
		public static readonly string VPN4SolutionFileName = Path.Combine(BaseDirName, "VPN4.sln");
		public static readonly string DebugSnapshotBaseDir = @"S:\SE4\DebugFilesSnapshot";
		public static readonly string ReleaseDestDir = @"s:\SE4\Releases";
		public const string Prefix = "";
#else
#if !BU_OSS
		// SoftEther VPN (build by SoftEther)
		public static readonly string VPN4SolutionFileName = Path.Combine(BaseDirName, "SEVPN.sln");
		public static readonly string DebugSnapshotBaseDir = @"S:\SE4\DebugFilesSnapshot_SEVPN";
		public static readonly string ReleaseDestDir = @"s:\SE4\Releases_SEVPN";
		public const string Prefix = "softether-";
#else
		// SoftEther VPN (build by Open Source Developers)
		public static readonly string VPN4SolutionFileName = Path.Combine(BaseDirName, "SEVPN.sln");
		public static readonly string DebugSnapshotBaseDir = IO.NormalizePath(Path.Combine(BaseDirName, @"..\output\debug"));
		public static readonly string ReleaseDestDir = IO.NormalizePath(Path.Combine(BaseDirName, @"..\output\pkg"));
		public const string Prefix = "softether_open-";
#endif
#endif

		public static readonly string ReleaseDestDir_SEVPN = @"s:\SE4\Releases_SEVPN";

		public static readonly string BuildHamcoreFilesDirName = Path.Combine(BinDirName, "BuiltHamcoreFiles");
		public static readonly string VisualStudioVCDir;
		public static readonly string VisualStudioVCBatchFileName;
		public static readonly string DotNetFramework35Dir;
		public static readonly string MSBuildFileName;
		public static readonly string TmpDirName;
		public static readonly DateTime StartDateTime = DateTime.Now;
		public static readonly string StartDateTimeStr;
		public static readonly string CmdFileName;
		public static readonly string ManifestsDir = Path.Combine(BaseDirName, @"BuildFiles\Manifests");
		public static readonly string XCopyExeFileName = Path.Combine(Env.SystemDir, "xcopy.exe");
		public static readonly string ReleaseDir = Path.Combine(BaseDirName, @"tmp\Release");
		public static readonly string ReleaseSrckitDir = Path.Combine(BaseDirName, @"tmp\ReleaseSrcKit");
		public static readonly string StringsDir = Path.Combine(BaseDirName, @"BuildFiles\Strings");
		public static readonly string CrossCompilerBaseDir = @"S:\CommomDev\xc";
		public static readonly string UnixInstallScript = Path.Combine(BaseDirName, @"BuildFiles\UnixFiles\InstallScript.txt");
		public static readonly string OssCommentsFile = Path.Combine(StringsDir, "OssComments.txt");
		public static readonly string AutorunSrcDir = IO.NormalizePath(Path.Combine(BaseDirName, @"..\Autorun"));
		public static readonly string MicrosoftSDKDir;
		public static readonly string MakeCatFilename;
		public static readonly string RcFilename;
		public static readonly string SoftEtherBuildDir = Env.SystemDir.Substring(0, 2) + @"\tmp\softether_build_dir";
		public static readonly string OpenSourceDestDir = Env.SystemDir.Substring(0, 2) + @"\tmp\softether_oss_dest_dir";

		// Initialize
		static Paths()
		{
			// Starting date and time string
			Paths.StartDateTimeStr = Str.DateTimeToStrShort(Paths.StartDateTime);

			// Check whether the execution path is the bin directory in the VPN directory
			if (Paths.BinDirName.EndsWith(@"\bin", StringComparison.InvariantCultureIgnoreCase) == false)
			{
				throw new ApplicationException(string.Format("'{0}' is not a VPN bin directory.", Paths.BinDirName));
			}
			if (File.Exists(Paths.VPN4SolutionFileName) == false)
			{
				throw new ApplicationException(string.Format("'{0}' is not a VPN base directory.", Paths.BaseDirName));
			}

			// Get the VC++ directory
			// Visual Studio 2008
			if (IntPtr.Size == 4)
			{
				Paths.VisualStudioVCDir = IO.RemoteLastEnMark(Reg.ReadStr(RegRoot.LocalMachine, @"SOFTWARE\Microsoft\VisualStudio\9.0\Setup\VC", "ProductDir"));
			}
			else
			{
				Paths.VisualStudioVCDir = IO.RemoteLastEnMark(Reg.ReadStr(RegRoot.LocalMachine, @"SOFTWARE\Wow6432Node\Microsoft\VisualStudio\9.0\Setup\VC", "ProductDir"));
			}
			if (Str.IsEmptyStr(Paths.VisualStudioVCDir))
			{
				throw new ApplicationException("Visual C++ directory not found.\n");
			}
			if (Directory.Exists(Paths.VisualStudioVCDir) == false)
			{
				throw new ApplicationException(string.Format("Directory '{0}' not found.", Paths.VisualStudioVCDir));
			}

			// Get the VC++ batch file name
			Paths.VisualStudioVCBatchFileName = Path.Combine(Paths.VisualStudioVCDir, "vcvarsall.bat");
			if (File.Exists(Paths.VisualStudioVCBatchFileName) == false)
			{
				throw new ApplicationException(string.Format("File '{0}' not found.", Paths.VisualStudioVCBatchFileName));
			}

			bool x86_dir = false;

			// Get Microsoft SDK 6.0a directory
			if (IntPtr.Size == 4)
			{
				Paths.MicrosoftSDKDir = IO.RemoteLastEnMark(Reg.ReadStr(RegRoot.LocalMachine, @"SOFTWARE\Wow6432Node\Microsoft\Microsoft SDKs\Windows\v6.0A", "InstallationFolder"));
			}
			else
			{
				Paths.MicrosoftSDKDir = IO.RemoteLastEnMark(Reg.ReadStr(RegRoot.LocalMachine, @"SOFTWARE\Microsoft\Microsoft SDKs\Windows\v6.0A", "InstallationFolder"));
			}

			// Get makecat.exe file name
			Paths.MakeCatFilename = Path.Combine(Paths.MicrosoftSDKDir, @"bin\" + (x86_dir ? @"x86\" : "") + "makecat.exe");

			// Get the rc.exe file name
			Paths.RcFilename = Path.Combine(Paths.MicrosoftSDKDir, @"bin\" + (x86_dir ? @"x86\" : "") + "rc.exe");

			// Get the cmd.exe file name
			Paths.CmdFileName = Path.Combine(Env.SystemDir, "cmd.exe");
			if (File.Exists(Paths.CmdFileName) == false)
			{
				throw new ApplicationException(string.Format("File '{0}' not found.", Paths.CmdFileName));
			}

			// Get .NET Framework 3.5 directory
			Paths.DotNetFramework35Dir = Path.Combine(Env.WindowsDir, @"Microsoft.NET\Framework\v3.5");

			// Get msbuild.exe directory
			Paths.MSBuildFileName = Path.Combine(Paths.DotNetFramework35Dir, "MSBuild.exe");
			if (File.Exists(Paths.MSBuildFileName) == false)
			{
				throw new ApplicationException(string.Format("File '{0}' not found.", Paths.MSBuildFileName));
			}

			// Get the TMP directory
			Paths.TmpDirName = Path.Combine(Paths.BaseDirName, "tmp");
			if (Directory.Exists(Paths.TmpDirName) == false)
			{
				Directory.CreateDirectory(Paths.TmpDirName);
			}
		}

		public static void DeleteAllReleaseTarGz()
		{
			if (Directory.Exists(Paths.ReleaseDir))
			{
				string[] files = Directory.GetFiles(Paths.ReleaseDir, "*.gz", SearchOption.AllDirectories);

				foreach (string file in files)
				{
					File.Delete(file);
				}
			}

			if (Directory.Exists(Paths.ReleaseSrckitDir))
			{
				string[] files = Directory.GetFiles(Paths.ReleaseSrckitDir, "*.gz", SearchOption.AllDirectories);

				foreach (string file in files)
				{
					File.Delete(file);
				}
			}
		}

		public static void DeleteAllReleaseAdminKits()
		{
			if (Directory.Exists(Paths.ReleaseDir))
			{
				string[] files = Directory.GetFiles(Paths.ReleaseDir, "*.zip", SearchOption.AllDirectories);

				foreach (string file in files)
				{
					if (Str.InStr(file, "vpnadminpak"))
					{
						File.Delete(file);
					}
				}
			}
		}

		public static void DeleteAllReleaseManuals()
		{
			if (Directory.Exists(Paths.ReleaseDir))
			{
				string[] files = Directory.GetFiles(Paths.ReleaseDir, "*", SearchOption.AllDirectories);

				foreach (string file in files)
				{
					if (Str.InStr(file, "vpnmanual"))
					{
						File.Delete(file);
					}
				}
			}
		}

		public static void DeleteAllReleaseExe()
		{
			if (Directory.Exists(Paths.ReleaseDir))
			{
				string[] files = Directory.GetFiles(Paths.ReleaseDir, "*.exe", SearchOption.AllDirectories);

				foreach (string file in files)
				{
					if (Str.InStr(file, "vpnmanual") == false)
					{
						File.Delete(file);
					}
				}
			}
		}
	}

	// HamCore build utility
	public static class HamCoreBuildUtil
	{
		// Identify whether a file is necessary only in the Win32
		public static bool IsFileForOnlyWin32(string filename)
		{
			string[] filesOnlyWin32 =
			{
				".exe",
				".dll",
				".sys",
				".inf",
				".wav",
				".cat",
			};

			foreach (string ext in filesOnlyWin32)
			{
				if (filename.EndsWith(ext, StringComparison.InvariantCultureIgnoreCase))
				{
					return true;
				}
			}

			return false;
		}

		// Delete svn file
		public static void DeleteSVNFilesFromHamCoreBuilder(HamCoreBuilder b)
		{
			List<string> removeFiles = new List<string>();
			foreach (HamCoreBuilderFileEntry f in b.FileList)
			{
				string name = f.Name;
				if (name.StartsWith(".svn", StringComparison.InvariantCultureIgnoreCase) ||
					name.IndexOf(@"\.svn", StringComparison.InvariantCultureIgnoreCase) != -1)
				{
					removeFiles.Add(name);
				}
			}
			foreach (string file in removeFiles)
			{
				b.DeleteFile(file);
			}
		}

		// Build Hamcore file
		public static void BuildHamcore()
		{
			string srcDirNameBasic = Path.Combine(Paths.BinDirName, "hamcore");
			// Create the destination directory
			string win32DestDir = Path.Combine(Paths.BuildHamcoreFilesDirName, "hamcore_win32");
			string win32DestFileName = Path.Combine(win32DestDir, "hamcore.se2");
			string unixDestDir = Path.Combine(Paths.BuildHamcoreFilesDirName, "hamcore_unix");
			string unixDestFileName = Path.Combine(unixDestDir, "hamcore.se2");
			IO.MakeDir(win32DestDir);
			IO.MakeDir(unixDestDir);


			BuildHamcoreEx(srcDirNameBasic, win32DestFileName, unixDestFileName);

			// Copy to bin\hamcore.se2
			try
			{
				string binHamcoreFileName = Path.Combine(Paths.BinDirName, "hamcore.se2");

				try
				{
					File.Delete(binHamcoreFileName);
				}
				catch
				{
				}

				File.Copy(win32DestFileName, binHamcoreFileName, true);
			}
			catch
			{
			}
		}

		public static void BuildHamcoreEx(string srcDirNameBasic, string win32DestFileName, string unixDestFileName)
		{
			HamCoreBuilder b = new HamCoreBuilder();
			b.AddDir(srcDirNameBasic);
			Con.WriteLine("* Building hamcore ...");

			DeleteSVNFilesFromHamCoreBuilder(b);

			try
			{
				File.Delete(win32DestFileName);
			}
			catch
			{
			}
			b.Build(win32DestFileName);

			// unix
			List<string> removeFiles = new List<string>();
			foreach (HamCoreBuilderFileEntry f in b.FileList)
			{
				if (IsFileForOnlyWin32(f.Name))
				{
					removeFiles.Add(f.Name);
				}
			}
			foreach (string removeFile in removeFiles)
			{
				b.DeleteFile(removeFile);
			}

			DeleteSVNFilesFromHamCoreBuilder(b);

			try
			{
				File.Delete(unixDestFileName);
			}
			catch
			{
			}
			b.Build(unixDestFileName);
		}
	}

	// Number of bits
	public enum CPUBits
	{
		Both,
		Bits32,
		Bits64,
	}

	// Conversion a string to the number of bits
	public static class CPUBitsUtil
	{
		public static CPUBits StringToCPUBits(string str)
		{
			if (str.Equals("32bit", StringComparison.InvariantCultureIgnoreCase))
			{
				return CPUBits.Bits32;
			}
			else if (str.Equals("64bit", StringComparison.InvariantCultureIgnoreCase))
			{
				return CPUBits.Bits64;
			}
			else if (str.Equals("intel", StringComparison.InvariantCultureIgnoreCase))
			{
				return CPUBits.Both;
			}

			throw new ApplicationException(string.Format("Invalid bits string '{0}'.", str));
		}

		public static string CPUBitsToString(CPUBits bits)
		{
			switch (bits)
			{
				case CPUBits.Bits32:
					return "32bit";

				case CPUBits.Bits64:
					return "64bit";

				case CPUBits.Both:
					return "intel";
			}

			throw new ApplicationException("bits invalid.");
		}
	}
}


