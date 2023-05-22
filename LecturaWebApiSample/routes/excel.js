const express = require('express');
const router = express.Router();
const XLSX = require('xlsx');

router.get('/excel', () => {});

function leerArchivoExcel(rutaArchivo) {
    // Cargar el archivo Excel
    const libro = XLSX.readFile(rutaArchivo);
    
    // Crear un objeto para almacenar las hojas
    const hojas = {};
    
    // Obtener el nombre de todas las hojas en el archivo
    const nombresHojas = libro.SheetNames;
    
    // Leer cada hoja y guardarla en el objeto 'hojas'
    nombresHojas.forEach(nombre => {
      const hoja = libro.Sheets[nombre];
      
      // Obtener los datos de la hoja y guardarlos en el objeto 'hojas'
      const datos = XLSX.utils.sheet_to_json(hoja, { header: 1 });
      hojas[nombre] = datos;
    })
      // Devolver el objeto 'hojas'
  return hojas;
};

// Ruta del archivo Excel
const rutaArchivoExcel = './public/Presupuestador-Importaciones-ARG.xlsx';

// Llamar a la función para leer el archivo Excel
const resultado = leerArchivoExcel(rutaArchivoExcel);

// Imprimir el contenido del objeto 'hojas'
for (const nombre in resultado) {
  console.log(`Hoja: ${nombre}`);
  resultado[nombre].forEach(fila => {
    console.log(fila);
  });
  console.log();
};

module.exports = router;