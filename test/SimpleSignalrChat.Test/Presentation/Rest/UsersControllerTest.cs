using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SimpleSignalrChat.BusinessLogic.DTOs;
using SimpleSignalrChat.DataAccess.Entities;
using SimpleSignalrChat.Presentaion.Rest.Requests;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace SimpleSignalrChat.Test.Presentation.Rest;

public class UsersControllerTest : ApiControllerTest
{
	[Fact]
	public async Task Get_UserExists_ShouldReturnUserInfo()
	{
		var user = new User { Name = "test user" };
		DbContext.Users.Add(user);
		DbContext.SaveChanges();

		Client.DefaultRequestHeaders.Add("userId", user.Id.ToString());
		var response = await Client.GetAsync($"api/users/{user.Id}");
		response.EnsureSuccessStatusCode();
		var data = JsonConvert.DeserializeObject<UserDto>(await response.Content.ReadAsStringAsync());

		Assert.NotNull(data);
		Assert.Equal(user.Name, data.Name);
	}

	[Fact]
	public async Task Get_UserDoesntExist_ShouldReturn404NotFound()
	{
		var nonexistentUserId = -999;

		var response = await Client.GetAsync($"api/users/{nonexistentUserId}");
		Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

		var details = JsonConvert.DeserializeObject<ProblemDetails>(await response.Content.ReadAsStringAsync());

		Assert.NotNull(details);
		Assert.Equal(404, details.Status);
	}

	[Fact]
	public async Task Delete_UserExists_ShouldReturnSuccess()
	{
		var user = new User { Name = "test user" };
		DbContext.Users.Add(user);
		DbContext.SaveChanges();

		Client.DefaultRequestHeaders.Add("Authorization", user.Id.ToString());
		var response = await Client.DeleteAsync($"api/users");

		response.EnsureSuccessStatusCode();
	}


	[Fact]
	public async Task Delete_UserDoesntExist_ShouldReturn404NotFound()
	{
		var nonexistentUserId = -999;

		Client.DefaultRequestHeaders.Add("Authorization", nonexistentUserId.ToString());
		var response = await Client.DeleteAsync($"api/users");

		Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

		var details = JsonConvert.DeserializeObject<ProblemDetails>(await response.Content.ReadAsStringAsync());

		Assert.NotNull(details);
		Assert.Equal(404, details.Status);
	}

	[Fact]
	public async Task Post_ValidInput_ShouldReturnCreatedUser()
	{
		var userName = "new user";

		var response = await Client.PostAsJsonAsync("api/users", new CreateUserRequest(userName));
		response.EnsureSuccessStatusCode();
		var data = JsonConvert.DeserializeObject<UserDto>(await response.Content.ReadAsStringAsync());

		Assert.NotNull(data);
		Assert.Equal(userName, data.Name);
	}
}
