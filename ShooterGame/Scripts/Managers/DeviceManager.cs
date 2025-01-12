using System;
using System.Runtime.InteropServices;

public static class DeviceManager
{
    [DllImport("depthai-unity", CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr GetAllDevices();

}