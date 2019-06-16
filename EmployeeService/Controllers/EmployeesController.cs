using EmployeeDataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web.Http;
using System.Web.Http.Cors;

namespace EmployeeService.Controllers
{
    //[EnableCorsAttribute("*","*","*")]
 
       public class EmployeesController : ApiController
    {
        [BasicAuthentication]
        public HttpResponseMessage Get(string Gender="All")
        {
            string username = Thread.CurrentPrincipal.Identity.Name;

            using (EmployeeDBEntities entities = new EmployeeDBEntities())
            {
                switch (username.ToLower())
                {
                    case "all":
                        return Request.CreateResponse(HttpStatusCode.OK, entities.Employees.ToList());
                    case "male":
                        return Request.CreateResponse(HttpStatusCode.OK, entities.Employees.Where(e => e.Gender == "male").ToList());
                    case "female":
                        return Request.CreateResponse(HttpStatusCode.OK, entities.Employees.Where(e => e.Gender == "female").ToList());
                    default:
                        return Request.CreateResponse(HttpStatusCode.BadRequest);
                       // return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Value for Gender must be Male,Female or All." +Gender+ " is Invalid.");
                }
                
            }
        }

        //[HttpGet]
        //public IEnumerable<Employee> Test()
        //{
        //    using (EmployeeDBEntities entities = new EmployeeDBEntities())
        //    {
        //        return entities.Employees.ToList();
        //    }
        //}
        //[RequireHttps]
        public HttpResponseMessage Get(int id)
        {
            using (EmployeeDBEntities entities = new EmployeeDBEntities())
            {
                var employee= entities.Employees.FirstOrDefault(e => e.ID == id);
                if (employee != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,employee);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Employee with Id:" + id +" not found");
                }
            }
        }

        public HttpResponseMessage Post([FromBody] Employee employee)
        {
            try
            {
                using (EmployeeDBEntities entities = new EmployeeDBEntities())
                {
                    entities.Employees.Add(employee);
                    entities.SaveChanges();
                    var Messsage = Request.CreateResponse(HttpStatusCode.Created, employee);
                    Messsage.Headers.Location = new Uri(Request.RequestUri + employee.ID.ToString());
                    return Messsage;
                }
            }
            catch (Exception ex)
            {
               return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
            
        }

        public HttpResponseMessage Delete(int id)
        {
            try
            {
                using (EmployeeDBEntities entities = new EmployeeDBEntities())
                {
                    var employee = entities.Employees.FirstOrDefault(e => e.ID == id);
                    if (employee != null)
                    {
                        entities.Employees.Remove(employee);
                        entities.SaveChanges();
                        return Request.CreateResponse(HttpStatusCode.OK);
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound, "Id: " + id + " does not exists to delete");
                    }
                }              
            }
            catch (Exception ex)
            {

                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        public HttpResponseMessage Put(int id,[FromBody] Employee employee)
        {
            try
            {
                using (EmployeeDBEntities entities = new EmployeeDBEntities())
                {
                    var entity = entities.Employees.FirstOrDefault(e => e.ID == id);
                    {
                        if (entity != null)
                        {
                            entity.FirstName = employee.FirstName;
                            entity.LastName = employee.LastName;
                            entity.Gender = employee.Gender;
                            entity.Salary = employee.Salary;

                            entities.SaveChanges();

                            return Request.CreateResponse(HttpStatusCode.OK, "Employee with Id: " + id + " updated successfully");
                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.NotFound, "Employee with Id: " + id + " doesnot exists to update");
                        }
                    }
                }
                    
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);                
            }
        }
    }
}
