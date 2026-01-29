using Fido2NetLib.Objects;

namespace FidoAuthServer.Models.Entities;

public record User(string Username, string DisplayName, byte[] Id, List<PublicKeyCredentialDescriptor>? Credentials = null)
{
    public List<PublicKeyCredentialDescriptor> Credentials { get; init; } = Credentials ?? [];
};
