// import React from 'react'
import { useAuth0 } from "@auth0/auth0-react";
import JSONPretty from 'react-json-pretty';
import 'react-json-pretty/themes/monikai.css'; // es opcional le da el estilo al JSONPretty

export const Profile = () => {
    const {user, isAuthenticated} = useAuth0();
  return (
    //valido que el usuario este autenticado para traerlo a la vista, sino rompe
        isAuthenticated && (
            <div>
                <img src={user.picture} alt={user.nickname}/>
                <h4>NickName: {user.nickname}</h4>
                <p>Name: {user.name}</p>
                <p>Email: {user.email}</p>

                <JSONPretty data={user}/>
            </div>
        )
  )
}
