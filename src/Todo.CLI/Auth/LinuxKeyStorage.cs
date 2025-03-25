using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Todo.CLI.Auth;

public class LinuxKeyStorage : IKeyStorage
{
    private const string SchemaName = "org.freedesktop.Secret.Generic";
    private const string Collection = "login";
    private const string Label = "Todo.CLI Encryption Key";
    private const string Attribute = "Todo.CLI.EncryptionKey";

    [DllImport("libsecret-1.so.0")]
    private static extern IntPtr secret_password_store_sync(
        string schema,
        string collection,
        string label,
        byte[] password,
        IntPtr cancellable,
        out IntPtr error,
        string attribute_name,
        string attribute_value,
        IntPtr end
    );

    [DllImport("libsecret-1.so.0")]
    private static extern IntPtr secret_password_lookup_sync(
        string schema,
        IntPtr cancellable,
        out IntPtr error,
        string attribute_name,
        string attribute_value,
        IntPtr end
    );

    [DllImport("libsecret-1.so.0")]
    private static extern void secret_password_free(IntPtr password);

    public byte[] GetKey()
    {
        var result = secret_password_lookup_sync(
            SchemaName,
            IntPtr.Zero,
            out IntPtr error,
            "attribute",
            Attribute,
            IntPtr.Zero
        );

        if (error != IntPtr.Zero)
        {
            throw new InvalidOperationException("Failed to retrieve key from Secret Service");
        }

        if (result == IntPtr.Zero)
        {
            throw new InvalidOperationException("Key not found in Secret Service");
        }

        try
        {
            var passwordStr = Marshal.PtrToStringAnsi(result);
            return Convert.FromBase64String(passwordStr!);
        }
        finally
        {
            secret_password_free(result);
        }
    }

    public void SaveKey(byte[] key)
    {
        var base64Key = Convert.ToBase64String(key);
        var keyBytes = Encoding.UTF8.GetBytes(base64Key);

        var result = secret_password_store_sync(
            SchemaName,
            Collection,
            Label,
            keyBytes,
            IntPtr.Zero,
            out IntPtr error,
            "attribute",
            Attribute,
            IntPtr.Zero
        );

        if (error != IntPtr.Zero || result == IntPtr.Zero)
        {
            throw new InvalidOperationException("Failed to save key to Secret Service");
        }
    }
} 