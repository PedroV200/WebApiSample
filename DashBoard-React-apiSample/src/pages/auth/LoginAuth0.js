import React from 'react';

//importo auth0
import { useAuth0 } from '@auth0/auth0-react';


const LoginAuth0 = () => {

    const { loginWithRedirect } = useAuth0();

  return (
    <>
        <h1>LoginAuth0</h1>
        <button onClick={loginWithRedirect()}>Login:</button>
    </>
  )
}

export default LoginAuth0;