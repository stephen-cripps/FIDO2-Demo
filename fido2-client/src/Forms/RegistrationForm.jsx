import React, { useState } from 'react';
import { RegisterUser } from '../Services/AuthenticationService';
import Button from 'react-bootstrap/Button';
import Form from 'react-bootstrap/Form';
import FloatingLabel from 'react-bootstrap/FloatingLabel';

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
      <Form onSubmit={handleSubmit}>
        <Form.Group className='mb-3' controlId='formUsername'>
          <FloatingLabel controlId='formUsername' label='Username' className='mb-3'>
            <Form.Control
              type='string'
              placeholder='Enter username'
              value={username}
              onChange={(e) => setUsername(e.target.value)}
            />
          </FloatingLabel>
        </Form.Group>

        <Form.Group className='mb-3' controlId='formDisplayName'>
          <FloatingLabel controlId='formDisplayName' label='Display Name' className='mb-3'>
            <Form.Control
              type='string'
              placeholder='Enter display name'
              value={displayName}
              onChange={(e) => setDisplayName(e.target.value)}
            />
          </FloatingLabel>
        </Form.Group>
        <Button variant='primary' type='submit'>
          Submit
        </Button>
      </Form>
    </div>
  );
};

export default RegistrationForm;
