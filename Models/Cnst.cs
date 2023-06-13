namespace WebApiSample.Models;


// Modelo de una entrada en la tabla constantes, 
// En la descripcion debe ir el nombre de la variable que se quiere asignar dentro de la clase CONSTANTES.
// En value el valor. Detalle es un campo optativo, de informacion interno.

public class Cnst
{
    public int id {get;set;}
    public string description{get;set;}
    public double val{get;set;}
    public string detalle{get;set;}

}