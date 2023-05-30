import { useContext } from 'react';
import { AuthContext } from 'src/contexts/auth-context';

//importo auth0
import { useAuth0 } from '@auth0/auth0-react';

export const useAuth = () => useContext(AuthContext);
