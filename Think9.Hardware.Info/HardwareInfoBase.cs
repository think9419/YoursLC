﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace Hardware.Info
{
    internal class HardwareInfoBase
    {
        internal static Process StartProcess(string cmd, string args)
        {
            ProcessStartInfo processStartInfo = new ProcessStartInfo(cmd, args)
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true
            };

            return Process.Start(processStartInfo);
        }

        internal static string ReadProcessOutput(string cmd, string args)
        {
            try
            {
                using Process process = StartProcess(cmd, args);
                using StreamReader streamReader = process.StandardOutput;
                process.WaitForExit();

                return streamReader.ReadToEnd().Trim();
            }
            catch
            {
                return string.Empty;
            }
        }

        internal static string TryReadFileText(string path)
        {
            try
            {
                return File.ReadAllText(path).Trim();
            }
            catch
            {
                return string.Empty;
            }
        }

        internal static string[] TryReadFileLines(string path)
        {
            try
            {
                return File.ReadAllLines(path);
            }
            catch
            {
                return Array.Empty<string>();
            }
        }

        public virtual List<Drive> GetDriveList()
        {
            List<Drive> driveList = new List<Drive>();

            Drive drive = new Drive();

            Partition partition = new Partition();

            foreach (DriveInfo driveInfo in DriveInfo.GetDrives())
            {
                Volume volume = new Volume
                {
                    FileSystem = driveInfo.DriveFormat,
                    Description = driveInfo.DriveType.ToString(),
                    Name = driveInfo.Name,
                    Caption = driveInfo.RootDirectory.FullName,
                    FreeSpace = (ulong)driveInfo.TotalFreeSpace,
                    Size = (ulong)driveInfo.TotalSize,
                    VolumeName = driveInfo.VolumeLabel
                };

                partition.VolumeList.Add(volume);
            }

            drive.PartitionList.Add(partition);

            driveList.Add(drive);

            return driveList;
        }

        public virtual List<NetworkAdapter> GetNetworkAdapterList(bool includeBytesPersec = true, bool includeNetworkAdapterConfiguration = true)
        {
            List<NetworkAdapter> networkAdapterList = new List<NetworkAdapter>();

            foreach (NetworkInterface networkInterface in NetworkInterface.GetAllNetworkInterfaces())
            {
                NetworkAdapter networkAdapter = new NetworkAdapter
                {
                    MACAddress = networkInterface.GetPhysicalAddress().ToString().Trim(),
                    Description = networkInterface.Description.Trim(),
                    Name = networkInterface.Name.Trim()
                };

                if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    networkAdapter.Speed = (ulong)networkInterface.Speed;
                }

                if (includeNetworkAdapterConfiguration)
                {
                    foreach (UnicastIPAddressInformation addressInformation in networkInterface.GetIPProperties().UnicastAddresses)
                    {
                        if (addressInformation.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            networkAdapter.IPAddressList.Add(addressInformation.Address);
                        }
                    }
                }

                networkAdapterList.Add(networkAdapter);
            }

            return networkAdapterList;
        }
    }
}