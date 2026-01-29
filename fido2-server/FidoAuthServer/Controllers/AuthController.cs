using System.Text;
using Fido2NetLib;
using Fido2NetLib.Objects;
using FidoAuthServer.Data;
using FidoAuthServer.Models.Requests;
using Microsoft.AspNetCore.Mvc;

namespace FidoAuthServer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IFido2 fido2, IUserRepository userRepo) : Controller
{
    // The client initially requests some options to set up the authenticator (I.E the keypair?) with
    [HttpPost("CredentialOptions")]
    public CredentialCreateOptions CreateCredentialOptions([FromBody] CredentialOptionsRequest request)
    {
        var userEntity = userRepo.GetByUsername(request.Username);
        
        // If existing user, we're adding an authenticator to their account use existing info, else use the info from the request to create a new user.
        var user = new Fido2User()
            {
                Name = request.Username, // Request username will always match the entity username if it exists
                DisplayName = userEntity?.DisplayName ?? request.DisplayName,
                Id = Encoding.UTF8.GetBytes(request.Username)
            };

        return fido2.RequestNewCredential(new RequestNewCredentialParams()
        {
            User = user,
            AuthenticatorSelection = AuthenticatorSelection.Default, // ToDo: What is this? 
            AttestationPreference = AttestationConveyancePreference.None, // ToDo: What is this? 
        });
    }

    // This takes the response from the client after they have set up their authenticator and creates/Updates the user in the DB
    [HttpPost("SubmitCredential")]
    public async Task SubmitCredential([FromBody] RegisterRequest request)
    {
        // Checks if the user already has an authen
        IsCredentialIdUniqueToUserAsyncDelegate callback =
            (args, cancellationToken) => Task.FromResult(userRepo.IsCredentialAlreadyUsed(args.CredentialId));
        
        var credential = await fido2.MakeNewCredentialAsync(new MakeNewCredentialParams()
        {
            AttestationResponse = request.Attestation,
            OriginalOptions = request.Options,
            IsCredentialIdUniqueToUserCallback = callback
        });

        var storedCredential = new PublicKeyCredentialDescriptor(PublicKeyCredentialType.PublicKey, credential.Id, credential.Transports);
        
        userRepo.AddCredentialToUser(request.Options.User, storedCredential);
    }
}
