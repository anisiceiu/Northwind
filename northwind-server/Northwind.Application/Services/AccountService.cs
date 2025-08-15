using AutoMapper;
using Northwind.Application.DTOs;
using Northwind.Application.Interfaces;
using Northwind.Core.Entities;
using Northwind.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Northwind.Application.Services
{
    public class AccountService : IAccountService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AccountService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<UserDto> Register(UserDto userDto, string password)
        {
            // Validate
            if (await _unitOfWork.AccountRepository.FindByCondition(u => u.Username == userDto.Username, false).AnyAsync())
                throw new Exception("Username already exists");

            if (await _unitOfWork.AccountRepository.FindByCondition(u => u.Email == userDto.Email, false).AnyAsync())
                throw new Exception("Email already exists");

            // Create password hash and salt
            CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);
            var user = _mapper.Map<User>(userDto);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            user.CreatedAt = DateTime.UtcNow;

            // Save user
            await _unitOfWork.AccountRepository.AddAsync(user);
            await _unitOfWork.CommitAsync();


            return _mapper.Map<UserDto>(user);
        }

        public async Task<UserDto> Login(string username, string password)
        {
            var user = await _unitOfWork.AccountRepository.FindByCondition(u => u.Email == username, false).FirstOrDefaultAsync();

            if (user == null)
                throw new Exception("User not found");

            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                throw new Exception("Invalid password");

            if (!user.IsActive)
                throw new Exception("User account is inactive");

            // Update last login
            user.LastLogin = DateTime.UtcNow;

            return _mapper.Map<UserDto>(user);
        }

        public bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            using (var hmac = new HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != storedHash[i]) return false;
                }
            }
            return true;
        }

        public void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

    }
}
