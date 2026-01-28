import React, { useState } from 'react';
import { RegisterUser } from '../Services/AuthenticationService';

const RegistrationForm = () => {
  const [displayName, setDisplayName] = useState('');
  const [username, setUsername] = useState('');

  const handleSubmit = (e) => {
    e.preventDefault();
    RegisterUser(username, displayName);
  };

  return (
    <div>
      <h2>Register</h2>
      <form onSubmit={handleSubmit}>
        <div>
          <label htmlFor='username'>Username:</label>
          <input
            type='text'
            id='username'
            name='username'
            required
            value={username}
            onChange={(e) => setUsername(e.target.value)}
          />
        </div>
        <div>
          <label htmlFor='displayName'>Display Name:</label>
          <input
            type='text'
            id='displayName'
            name='displayName'
            required
            value={displayName}
            onChange={(e) => setDisplayName(e.target.value)}
          />
        </div>
        <button type='submit'>Register</button>
      </form>
    </div>
  );
};

export default RegistrationForm;
