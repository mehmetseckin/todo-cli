using System;
using System.Runtime.InteropServices;

namespace Todo.CLI.Auth;

public static class KeyStorageFactory
{
    public static IKeyStorage Create()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return new WindowsKeyStorage();
        }
        
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            return new MacOSKeyStorage();
        }
        
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            return new LinuxKeyStorage();
        }

        throw new PlatformNotSupportedException("Current platform does not have a supported key storage implementation.");
    }
} 