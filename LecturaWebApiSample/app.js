const express = require('express');

// Crear una instancia de la aplicación Express
const app = express();

//Hago que public este disponible como un recurso estatico
app.use(express.static("public"));

//Agrego estas lineas para que pueda postear formularios
// app.use(methodOverride("_method"));
app.use(express.urlencoded({extended:false}));
app.use(express.json());

//requerimiento de rutas
const excel = require('./routes/excel.js');


// Definir una ruta
app.get('/', (req, res) => {
  res.send('¡Conexión exitosa a la ruta!');
});

// Puerto en el que se ejecutará el servidor
const puerto = 3000;

// Iniciar el servidor
app.listen(puerto, () => {
  console.log(`Servidor Express escuchando en el puerto ${puerto}`);
});
