﻿namespace BicycleApp.Services.Contracts.Factory
{   
    public interface IUserImageFactory
    {
        public Task<string?> GetUserImagePathAsync(string userId, string userRole);
        public Task<bool> CheckForExistingUserImage(string userId, string userRole);
        public Task<bool> UpdateUserImage(string userId, string userRole, string filePath);
        public Task<bool> CreateUserImage(string userId, string userRole, string filePath, string imageName);
    }
}
