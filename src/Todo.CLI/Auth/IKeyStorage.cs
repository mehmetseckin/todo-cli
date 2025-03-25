namespace Todo.CLI.Auth;

public interface IKeyStorage
{
    byte[] GetKey();
    void SaveKey(byte[] key);
} 