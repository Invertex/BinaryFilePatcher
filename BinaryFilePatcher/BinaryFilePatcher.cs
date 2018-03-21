using System;
using System.IO;
using System.Security;
using System.Security.Permissions;
using Invertex.BinaryFilePatcher.Extensions;

namespace Invertex.BinaryFilePatcher
{
    class BinaryFilePatcher
    {
        static void Main(string[] args)
        {
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
                            int replaced = ReplaceBytes(fileBytes, matchBytes, replaceBytes, replaceAllInstances);
                            if (replaced > 0)
                            {
                                File.WriteAllBytes(path, fileBytes);
                            }
                            Console.WriteLine("Found and replaced " + replaced + " instances of bytes: " + matchBytes.BytesToString("-"));
                        }
                        else
                        {
                            Console.WriteLine("Patcher failed.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Patcher lacks rights.");
                    }
                }
            }
            Console.ReadLine();
        }

        public static int ReplaceBytes(byte[] inBytes, byte[] matchBytes, byte[] replaceBytes, bool replaceAllInstances = false)
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
                else
                {
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
