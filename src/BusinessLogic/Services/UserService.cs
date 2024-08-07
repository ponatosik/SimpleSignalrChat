﻿using SimpleSignalrChat.BusinessLogic.Abstractions;
using SimpleSignalrChat.BusinessLogic.DTOs;
using SimpleSignalrChat.BusinessLogic.Exceptions.NotFound;
using SimpleSignalrChat.BusinessLogic.Services.Interfaces;
using SimpleSignalrChat.DataAccess.Entities;
using SimpleSignalrChat.DataAccess.Exceptions;
using SimpleSignalrChat.DataAccess.Repositories.Interfaces;

namespace SimpleSignalrChat.BusinessLogic.Services;

public class UserService: IUserService
{
	private readonly IUserRepository _userRepository;

	public UserService(IUserRepository userRepository)
	{
		_userRepository = userRepository;
	}

	public async Task<Result<UserDto>> GetUserAsync(int id) 
	{
		User? user = await _userRepository.GetUserAsync(id);
		if(user is null)
		{
			return new UserNotFoundException(id);
		}
		return UserDto.From(user!);
	}

	public async Task<Result<UserDto>> CreateUserAsync(string name)
	{
		User user = new User() { Name = name };
		return UserDto.From((await _userRepository.AddUserAsync(user))!);
	}

	public async Task<Result> DeleteUserAsync(int id)
	{
		try
		{
			await _userRepository.DeleteUserAsync(id);
			return Result.Success;
		}
		catch (EntityNotFoundException<User> exception)
		{
			return new UserNotFoundException (exception.EntityKey);
		}
	}
}
