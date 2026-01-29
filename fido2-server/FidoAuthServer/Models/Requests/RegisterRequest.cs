using Fido2NetLib;

namespace FidoAuthServer.Models.Requests;

public record RegisterRequest(AuthenticatorAttestationRawResponse Attestation, CredentialCreateOptions Options);
