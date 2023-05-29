import { useAuth0 } from '@auth0/auth0-react';
import './App.css';
import { LogOutButton } from './components/LogOutButton';
import { LoginButton } from './components/LoginButton';
import { Profile } from './components/Profile';


function App() {

  const {isAuthenticated, isLoading} = useAuth0();

  if(isLoading) return <h1>Loading...</h1>;

  return (
    <div className="App">
      <h2>Application</h2>
      {
        isAuthenticated ? <LogOutButton/> : <LoginButton/>
      }
      <Profile/>
    </div>
  );
}

export default App;
