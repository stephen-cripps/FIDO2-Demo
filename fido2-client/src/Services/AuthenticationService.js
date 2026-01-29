const baseUrl = 'https://localhost:7227/api/';

export const RegisterUser = async (username, displayName) => {
  const credentialOptions = await getCredentialOptions(username, displayName);
  console.log('Credential Options Response:', credentialOptions);

  const credential = await generateCredentials(credentialOptions);

  await SubmitCredential(credential, credentialOptions);
};

const getCredentialOptions = async (username, displayName) => {
  const url = baseUrl + 'auth/CredentialOptions';

  const requestBody = {
    username: username,
    displayName: displayName,
  };

  const credentialOptions = await fetch(url, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(requestBody),
  });

  if (!credentialOptions.ok) {
    throw new Error('Failed to get credential options');
  }

  return await credentialOptions.json();
};

const generateCredentials = async (options) => {
  const credential = await navigator.credentials.create({
    publicKey: {
      ...options,
      challenge: base64urlToBuffer(options.challenge),
      user: {
        ...options.user,
        id: base64urlToBuffer(options.user.id),
      },
    },
  });

  console.log('Created Credential:', credential);

  return credential;
};

const SubmitCredential = async (credential, options) => {
  const url = baseUrl + 'auth/SubmitCredential';

  const requestBody = {
    Attestation: credential,
    options: options,
  };

  const response = await fetch(url, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(requestBody),
  });

  if (!response.ok) {
    throw new Error('Failed to submit credential');
  }
};

function base64urlToBuffer(base64url) {
  const padding = '='.repeat((4 - (base64url.length % 4)) % 4);
  const base64 = (base64url + padding).replace(/-/g, '+').replace(/_/g, '/');

  return Uint8Array.from(atob(base64), (c) => c.charCodeAt(0));
}
