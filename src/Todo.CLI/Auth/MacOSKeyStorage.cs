using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Todo.CLI.Auth;

public class MacOSKeyStorage : IKeyStorage
{
    private const string ServiceName = "Todo.CLI";
    private const string AccountName = "EncryptionKey";

    [DllImport("/System/Library/Frameworks/Security.framework/Security")]
    private static extern int SecKeychainFindGenericPassword(
        IntPtr keychain,
        uint serviceNameLength,
        byte[] serviceName,
        uint accountNameLength,
        byte[] accountName,
        out uint passwordLength,
        out IntPtr passwordData,
        out IntPtr itemRef
    );

    [DllImport("/System/Library/Frameworks/Security.framework/Security")]
    private static extern int SecKeychainAddGenericPassword(
        IntPtr keychain,
        uint serviceNameLength,
        byte[] serviceName,
        uint accountNameLength,
        byte[] accountName,
        uint passwordLength,
        byte[] passwordData,
        IntPtr itemRef
    );

    [DllImport("/System/Library/Frameworks/Security.framework/Security")]
    private static extern int SecKeychainItemFreeContent(
        IntPtr attrList,
        IntPtr data
    );

    public byte[] GetKey()
    {
        var serviceNameBytes = Encoding.UTF8.GetBytes(ServiceName);
        var accountNameBytes = Encoding.UTF8.GetBytes(AccountName);

        var result = SecKeychainFindGenericPassword(
            IntPtr.Zero,
            (uint)serviceNameBytes.Length,
            serviceNameBytes,
            (uint)accountNameBytes.Length,
            accountNameBytes,
            out uint passwordLength,
            out IntPtr passwordData,
            out IntPtr itemRef
        );

        if (result == 0) // Success
        {
            try
            {
                var key = new byte[passwordLength];
                Marshal.Copy(passwordData, key, 0, (int)passwordLength);
                return key;
            }
            finally
            {
                SecKeychainItemFreeContent(IntPtr.Zero, passwordData);
            }
        }

        throw new InvalidOperationException("Failed to retrieve key from Keychain");
    }

    public void SaveKey(byte[] key)
    {
        var serviceNameBytes = Encoding.UTF8.GetBytes(ServiceName);
        var accountNameBytes = Encoding.UTF8.GetBytes(AccountName);

        var result = SecKeychainAddGenericPassword(
            IntPtr.Zero,
            (uint)serviceNameBytes.Length,
            serviceNameBytes,
            (uint)accountNameBytes.Length,
            accountNameBytes,
            (uint)key.Length,
            key,
            IntPtr.Zero
        );

        if (result != 0)
        {
            throw new InvalidOperationException("Failed to save key to Keychain");
        }
    }
} 