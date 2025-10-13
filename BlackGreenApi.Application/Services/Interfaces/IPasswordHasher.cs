﻿namespace SaveNature.Services.Interfaces
{
    public interface IPasswordHasher
    {
        bool Verify(string password, string hash);
        string Hash(string password);
    }
}