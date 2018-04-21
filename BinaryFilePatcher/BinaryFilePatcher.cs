using System;
using System.IO;
using System.Security;
using System.Security.Permissions;
using Invertex.BinaryFilePatcher.Extensions;

namespace Invertex.BinaryFilePatcher
{
    class BinaryFilePatcher
    {
        static int Main(string[] args)
        {
            int exitCode = (int)ExitCodes.Success;

            if(args.Length >= 3)
            {
                string path = args[0];
                string matchString = args[1];
                string replaceString = args[2];
                bool replaceAllInstances = (args.Length >= 4 && string.Equals(args[3], "true", StringComparison.OrdinalIgnoreCase));

                Console.WriteLine("");
                Console.WriteLine("Binary File Patcher starting...");
                Console.WriteLine("");

                if(File.Exists(path))
                {
                    var permissionSet = new PermissionSet(PermissionState.None);
                    var writePermission = new FileIOPermission(FileIOPermissionAccess.Write, path);
                    permissionSet.AddPermission(writePermission);

                    if (permissionSet.IsSubsetOf(AppDomain.CurrentDomain.PermissionSet))
                    {
                        byte[] fileBytes = File.ReadAllBytes(path);
                        byte[] matchBytes = matchString.HexStringToBytes();
                        byte[] replaceBytes = replaceString.HexStringToBytes();

                        if (matchBytes != null && matchBytes.Length > 0
                            && replaceBytes != null && replaceBytes.Length == matchBytes.Length
                            && fileBytes != null && fileBytes.Length >= matchBytes.Length)
                        {
                            exitCode = ReplaceBytes(ref fileBytes, matchBytes, replaceBytes, replaceAllInstances);
                            if (exitCode > 0)
                            {
                                File.WriteAllBytes(path, fileBytes);
                            }
                            Console.WriteLine("Found and replaced " + exitCode + " instances of bytes: " + matchString.FormatHexString("-"));
                        }
                        else
                        {
                            Console.WriteLine("Patcher failed. Likely a mismatch of length between Match and Replace bytes.");
                            exitCode = (int)ExitCodes.MatchAndReplaceLengthMismatch;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Patcher lacks rights to access file: " + path);
                        Console.WriteLine("Must run this console as Administrator or move target file to folder that doesn't require Administrator rights to modify.");
                        exitCode = (int)ExitCodes.AdministrativeRightsRequired;
                    }
                }
                else
                {
                    Console.WriteLine("Couldn't find file: " + path);
                    exitCode = (int)ExitCodes.TargetFileNotFound;
                }
            }
            Console.ReadLine();
            return exitCode;
        }

        [Flags]
        enum ExitCodes : int
        {
            Success = 0, //0 and greater is success, with higher numbers being how many replacements
            NotEnoughArguments = -1,
            TargetFileNotFound = -2,
            AdministrativeRightsRequired = -4,
            MatchAndReplaceLengthMismatch = -8
        }

        public static int ReplaceBytes(ref byte[] inBytes, byte[] matchBytes, byte[] replaceBytes, bool replaceAllInstances = false)
        {
            int matchLength = matchBytes.Length;
            int curMatch = 0;
            int instancesReplaced = 0;

            for(int i = 0; i < inBytes.Length; i++)
            {
                if(inBytes[i] == matchBytes[curMatch])
                {
                    curMatch++;
                    if (curMatch == matchLength)
                    {
                        ReplaceByteRange(ref inBytes, replaceBytes, i - (curMatch - 1));
                        instancesReplaced++;
                        if (replaceAllInstances)
                        {
                            curMatch = 0;
                        }
                        else
                        {
                            return 1;
                        }
                    }
                }
                else if(curMatch > 0)
                { //If we failed after already matching some, we should make sure current isn't the start of a new match series before moving on
                    curMatch = (inBytes[i] == matchBytes[0]) ? 1 : 0;
                }
            }

            return instancesReplaced;
        }

        public static void ReplaceByteRange(ref byte[] bytes, byte[] replaceBytes, int start)
        {
            for (int i = 0; i < replaceBytes.Length; i++)
            {
                bytes[start + i] = replaceBytes[i];
            }
        }
    }
}
