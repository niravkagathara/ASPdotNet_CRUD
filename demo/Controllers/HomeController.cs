using System.Data.SqlClient;
using System.Data;
using System.Diagnostics;
using demo.Models;
using Microsoft.AspNetCore.Mvc;

namespace demo.Controllers
{
    public class HomeController : Controller
    {
        private IConfiguration configuration;

        public HomeController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }

        public IActionResult Index()
        {
            string connectionString = configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "PR_Customer_SelectAll";
            SqlDataReader reader = command.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);
            return View(table);
        }

        [HttpPost]
        public IActionResult CustomerDelete(int Customer_id)
        {
            try
            {
                string connectionString = configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "PR_Customer_Delete";
                command.Parameters.Add("@Customer_id", SqlDbType.Int).Value = Customer_id;
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                Console.WriteLine(ex.ToString());
            }
            return RedirectToAction("Index","Home");
        }


        public IActionResult ProductAddEdit(int? Customer_id)
        {
            string connectionString = this.configuration.GetConnectionString("ConnectionString");

            CustomerModel model = new CustomerModel();
            #region User Drop-Down

            //SqlConnection connection1 = new SqlConnection(connectionString);
            //connection1.Open();
            //SqlCommand command1 = connection1.CreateCommand();
            //command1.CommandType = System.Data.CommandType.StoredProcedure;
            //command1.CommandText = "PR_User_DropDown";
            //SqlDataReader reader1 = command1.ExecuteReader();
            //DataTable dataTable1 = new DataTable();
            //dataTable1.Load(reader1);
            //connection1.Close();

            //List<UserDropDownModel> users = new List<UserDropDownModel>();

            //foreach (DataRow dataRow in dataTable1.Rows)
            //{
            //    UserDropDownModel userDropDownModel = new UserDropDownModel();
            //    userDropDownModel.UserID = Convert.ToInt32(dataRow["UserID"]);
            //    userDropDownModel.UserName = dataRow["UserName"].ToString();
            //    users.Add(userDropDownModel);
            //}

            //ViewBag.UserList = users;

            #endregion

            if (Customer_id !=  null) {
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "PR_Customer_SelectByPK";
                command.Parameters.AddWithValue("@Customer_id", Customer_id);
                SqlDataReader reader = command.ExecuteReader();
                DataTable table = new DataTable();
                table.Load(reader);
                
                foreach (DataRow dataRow in table.Rows)
                {
                    model.Customer_id = Convert.ToInt32(@dataRow["Customer_id"]);
                    model.Customer_Name = @dataRow["Customer_Name"].ToString();
                    model.Email = @dataRow["Email"].ToString();
                }
            }

            #region ProductByID

            

            #endregion

            return View("ProductAddEdit", model);
        }

        [HttpPost]
        public IActionResult ProductAddEdit(CustomerModel model)
        {
            if (ModelState.IsValid)
            {
                string connectionString = this.configuration.GetConnectionString("ConnectionString");

                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                if (model.Customer_id == 0)
                {
                    command.CommandText = "PR_Customer_Insert";
                }
                else
                {
                    command.CommandText = "PR_Customer_Update";
                    command.Parameters.Add("@Customer_id", SqlDbType.Int).Value = model.Customer_id;
                }
                command.Parameters.Add("@Customer_Name", SqlDbType.VarChar).Value = model.Customer_Name;
                command.Parameters.Add("@Email", SqlDbType.VarChar).Value = model.Email;


                if (Convert.ToBoolean(command.ExecuteNonQuery()))
                {
                    TempData["InsertUpdateMSG"] = model.Customer_id == 0 ? "Inserted successfully!" : "Updated successfully!";
                    connection.Close();
                    return RedirectToAction("Index");
                }
                connection.Close();
            }
            return View("ProductAddEdit", model);
        }
        

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
