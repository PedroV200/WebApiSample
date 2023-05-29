import { useAuth0 } from "@auth0/auth0-react";

// import React from 'react';

export const LogOutButton = () => {
    const { logout } = useAuth0();
  return (
    <div>
        <button onClick={() => logout({ logoutParams: { returnTo: window.location.origin } })}>
      Log Out
        </button>
    </div>
  )
}
