using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
//definimos los imports necesarios
using System.Data.SqlClient;
using System.Data;
using System.Configuration; //vincula al Web.Config (alli tenemos nuestra conexion con el sql server 2014)
using Ejercicio02.Models; //importa las entidades - clases

namespace Ejercicio02.Controllers
{
    public class NegocioController : Controller
    {

        SqlConnection cn = new SqlConnection(
        ConfigurationManager.ConnectionStrings["cn"].ConnectionString);
        //ConfigurationManager obtiene los datos del connectionStrings para la configuracion de la aplicacion
        //cn sera nuestra conexion
        IEnumerable<Cliente> listado()
        {
            List<Cliente> temporal = new List<Cliente>();
            //SqlCommand representa una instruccion TRANSACT SQL o un proc. almacenado para ejecutar de una bd
            SqlCommand cmd = new SqlCommand("sp_clientes",cn); // la cadena con la sentencia sql o procedure  (CommandText)
            cmd.CommandType = CommandType.StoredProcedure; // CommandType especifico el tipo de 
            cn.Open(); // Abrir la conexion BD
            //SqlDataReader lee una secuencia de filas de solo avance de una bd
            SqlDataReader dr = cmd.ExecuteReader();// Execute reader envia el CommandText a la conexion(cn) y crea un objeto sqlDataReader
            while(dr.Read()) // El metodo read de SqlDataReader avanza a este al sgte registro
            {
                Cliente reg = new Cliente();//Se instancia la clase Cliente
                reg.idCliente = dr.GetString(0);
                reg.nombre = dr.GetString(1);
                reg.pais = dr.GetString(2);
                reg.telefono = dr.GetString(3);
                temporal.Add(reg);//Se llena la lista x cada fila qu ese va leyendo, es decir,se llena el cliente 1x1
            }
            dr.Close();// Se debe cerrar , pues mientras este no se haya cerrado, no se puede realizar otras operaciones en Sql Connection
            cn.Close();
            return temporal; // Retornamos la lista con los clientes
        }

        // GET: Negocio
        //VISTA : A la hora de agregar la vista la configuracion es :
        //                  Plantilla :LIST                 
        //                  Clase de Modelo : Cliente.Model
        public ActionResult Clientes()
        {
            return View(listado());
        }

        //Paginacion
        public ActionResult PaginacionClientes(int p=0)
        {
            //Recuperar Registros de la consulta : Lista General
            IEnumerable<Cliente> temporal = listado();

            //Definir el # de registros por pagina
            int num = 15;
      /*      int inicio=0;
            int final = num;*/
            //Definir el nro de paginas (Botones)
            int c = temporal.Count();
            int pags = c % num > 0 ? c / num + 1 : c / num;

            ViewBag.pags = pags;
            ViewBag.p = p; // Almaceno el nmro de la pagina 
            
            return View(temporal.Skip(p * num).Take(num));
        }

 
        IEnumerable<Cliente> listado_filtro(String nombre)
        {
            List<Cliente> temporal = new List<Cliente>();
            //SqlCommand representa una instruccion TRANSACT SQL o un proc. almacenado para ejecutar de una bd
            SqlCommand cmd = new SqlCommand("sp_clientes_nombre", cn); // la cadena con la sentencia sql o procedure  (CommandText)
            cmd.CommandType = CommandType.StoredProcedure; // CommandType especifico el tipo de 
            cmd.Parameters.AddWithValue("@nombre", nombre);
            cn.Open(); // Abrir la conexion BD
            //SqlDataReader lee una secuencia de filas de solo avance de una bd
            SqlDataReader dr = cmd.ExecuteReader();// Execute reader envia el CommandText a la conexion(cn) y crea un objeto sqlDataReader
            while (dr.Read()) // El metodo read de SqlDataReader avanza a este al sgte registro
            {
                Cliente reg = new Cliente();//Se instancia la clase Cliente
                reg.idCliente = dr.GetString(0);
                reg.nombre = dr.GetString(1);
                reg.direccion = dr.GetString(2);
                reg.pais = dr.GetString(3);
                reg.telefono = dr.GetString(4);
                temporal.Add(reg);//Se llena la lista x cada fila qu ese va leyendo, es decir,se llena el cliente 1x1
            }
            dr.Close();// Se debe cerrar , pues mientras este no se haya cerrado, no se puede realizar otras operaciones en Sql Connection
            cn.Close();
            return temporal; // Retornamos la lista con los clientes
        }

        //Paginacion con filtro

        public ActionResult PaginacionClientesFiltro(int p = 0, String nombre="")
        {
            //Recuperar Registros de la consulta : Lista General
            IEnumerable<Cliente> temporal = listado_filtro(nombre);

            //Definir el # de registros por pagina
            int num = 5;

            //Definir el nro de paginas (Botones)
            int c = temporal.Count();
            int pags = c % num > 0 ? c / num + 1 : c / num;

            ViewBag.pags = pags;
            ViewBag.nombre = nombre;
            ViewBag.p = p; // Almaceno el nmro de la pagina 

            return View(temporal.Skip(p * num).Take(num));//comeinza de la posicion p*num ,toma 5 paginas
        }

    }
}