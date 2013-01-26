using System;
using DotNetOpenAuth.OAuth2;
using Google.Apis.Authentication;
using Google.Apis.Authentication.OAuth2.DotNetOpenAuth;
using Google.Apis.Authentication.OAuth2;
using Google.Apis.Latitude.v1;
using Google.Apis.Samples.Helper;
using Google.Apis.Util;
using Google.Apis.Latitude.v1.Data;

namespace LatitudeClient
{
	class MainClass
	{
		//private static readonly string Scope = TasksService.Scopes.Tasks.GetStringValue();
		private static readonly string Scope = LatitudeService.Scopes.LatitudeCurrentBest.GetStringValue();

		public static void Main (string[] args)
		{
			// Register the authenticator.
			var provider = new NativeApplicationClient(GoogleAuthenticationServer.Description);
			FullClientCredentials credentials = PromptingClientCredentials.EnsureFullClientCredentials();
			provider.ClientIdentifier = credentials.ClientId;
			provider.ClientSecret = credentials.ClientSecret;

			var auth = new OAuth2Authenticator<NativeApplicationClient>(provider, GetAuthorization);

			// Create the service.
			var service = new LatitudeService(auth);

			var loc = service.CurrentLocation.Get().Fetch();

			Console.WriteLine("");

		}

		private static IAuthorizationState GetAuthorization(NativeApplicationClient client)
		{
			// You should use a more secure way of storing the key here as
			// .NET applications can be disassembled using a reflection tool.
			const string STORAGE = "latitude.client";
			const string KEY = "45dfgvq2345";
			
			// Check if there is a cached refresh token available.
			IAuthorizationState state = AuthorizationMgr.GetCachedRefreshToken(STORAGE, KEY);
			if (state != null)
			{
				try
				{
					client.RefreshToken(state);
					return state; // Yes - we are done.
				}
				catch (DotNetOpenAuth.Messaging.ProtocolException ex)
				{
					CommandLine.WriteError("Using existing refresh token failed: " + ex.Message);
				}
			}
			
			// Retrieve the authorization from the user.
			state = AuthorizationMgr.RequestNativeAuthorization(client, Scope);
			AuthorizationMgr.SetCachedRefreshToken(STORAGE, KEY, state);
			return state;
		}



	}
}
