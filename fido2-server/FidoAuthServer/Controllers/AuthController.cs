using System.Text;
using Fido2NetLib;
using Fido2NetLib.Objects;
using FidoAuthServer.Models;
using Microsoft.AspNetCore.Mvc;

namespace FidoAuthServer.Controllers;

public class AuthController(IFido2 fido2) : Controller
{
    // The client initially requests some options to set up the authernticator with? ToDo: verify. 
    [HttpPost]
    public void MakeCredentialOptions([FromBody] string username, string displayName)
    {
        var user = new Fido2User()
        {
            Name = username,
            DisplayName = displayName,
            Id = Encoding.UTF8.GetBytes(username)
        };

        var options = fido2.RequestNewCredential(new RequestNewCredentialParams()
        {
            User = user,
            AuthenticatorSelection = AuthenticatorSelection.Default, // ToDo: What is this? 
            AttestationPreference = AttestationConveyancePreference.None // ToDo: What is this? 
        });
        
        HttpContext.Session.SetString("fido2.attestationOptions", options.ToJson());
    }
}
