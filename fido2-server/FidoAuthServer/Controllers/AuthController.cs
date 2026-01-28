using System.Text;
using Fido2NetLib;
using Fido2NetLib.Objects;
using Microsoft.AspNetCore.Mvc;

namespace FidoAuthServer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IFido2 fido2) : Controller
{
    // The client initially requests some options to set up the authenticator (I.E the keypair?) with? ToDo: verify. 
    [HttpPost ("credentialOptions")]
    public CredentialCreateOptions CreateCredentialOptions([FromBody] CredentialOptionsRequest requestData)
    {
        var user = new Fido2User()
        {
            Name = requestData.username,
            DisplayName = requestData.displayName,
            Id = Encoding.UTF8.GetBytes(requestData.username)
        };

        return fido2.RequestNewCredential(new RequestNewCredentialParams()
        {
            User = user,
            AuthenticatorSelection = AuthenticatorSelection.Default, // ToDo: What is this? 
            AttestationPreference = AttestationConveyancePreference.None // ToDo: What is this? 
        });
    }
}
