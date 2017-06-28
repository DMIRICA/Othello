using Server.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.database
{
    class DatabaseConnection
    {
        string relativePath = @"..\..\database\database.sqlite3";
        string currentPath;
        string absolutePath;
        string connectionString;

        SQLiteConnection connection;

        public DatabaseConnection()
        {
            currentPath = AppDomain.CurrentDomain.BaseDirectory;
            absolutePath = System.IO.Path.Combine(currentPath, relativePath);
            connectionString = string.Format("DataSource={0}", absolutePath);
            connection = new SQLiteConnection(connectionString);
            Console.WriteLine("Connected to database!");
        }

        #region Services

        #region UserServices

        public bool addUser(User user)
        {
            bool done = false;
            //string hash = EncryptService.getHashFromString(user.Password);
            connection.Open();
            using (SQLiteCommand cmd = new SQLiteCommand(connection))
            {
                cmd.CommandText = "INSERT INTO USERS VALUES(@ID,@Username,@Password,@Email)";
                cmd.Prepare();
                cmd.Parameters.AddWithValue("@ID", null);
                cmd.Parameters.AddWithValue("@Username", user.Username);
                cmd.Parameters.AddWithValue("@Password", user.Password);
                cmd.Parameters.AddWithValue("@Email", user.Email);
                try
                {
                    done = (cmd.ExecuteNonQuery() == 1) ? true : false;
                }
                catch (SQLiteException e)
                {
                    Console.WriteLine("Add account exception ->" + e);
                }
                finally
                {
                    connection.Close();
                }
            }
            return done;
        }

        public User getUserByID(int ID)
        {
            User user = null;
            try
            {
                connection.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(connection))
                {
                    cmd.CommandText = "SELECT * FROM USERS WHERE ID=@ID";
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@ID", ID);
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            user = new User();
                            user.ID = Int32.Parse(reader["ID"].ToString());
                            user.Username = reader["Username"].ToString();
                            user.Password = reader["Password"].ToString();
                            user.Email = reader["Email"].ToString();
                            return user;
                        }
                    }
                }
            }
            catch (SQLiteException e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                connection.Close();
            }
            return user;
        }

        public User getUserByUsername(string Username)
        {
            User user = null;
            try
            {
                connection.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(connection))
                {
                    cmd.CommandText = "SELECT * FROM USERS WHERE Username=@Username";
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@Username", Username);
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            user = new User();
                            user.ID = Int32.Parse(reader["ID"].ToString());
                            user.Username = reader["Username"].ToString();
                            user.Password = reader["Password"].ToString();
                            user.Email = reader["Email"].ToString();
                            return user;
                        }
                    }
                }
            }
            catch (SQLiteException e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                connection.Close();
            }
            return user;
        }

        public List<User> getAllUsers()
        {
            List<User> list = new List<User>();
            try
            {

                connection.Open();
                string sql = "SELECT * FROM USERS;";
                using (SQLiteCommand cmd = new SQLiteCommand(sql, connection))
                {
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            User user = new User();
                            user.ID = Int32.Parse(reader["ID"].ToString());
                            user.Username = reader["Username"].ToString();
                            user.Password = reader["Password"].ToString();
                            user.Email = reader["Email"].ToString();
                            list.Add(user);
                        }
                    }
                }
                
            }

            catch (SQLiteException e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                connection.Close();
            }
            return list;
        }

        public bool isUsernameUsed(string Username)
        {
            bool used = false;
            try
            {
                connection.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(connection))
                {
                    cmd.CommandText = "SELECT * FROM USERS WHERE USERNAME=@Username";
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@Username", Username);
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)                        
                            used =  true;
                    }
                }
            }
            catch (SQLiteException e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                connection.Close();
            }
            return used;
        }

        public bool isEmailUsed(string Email)
        {
            bool used = false;
            try
            {
                connection.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(connection))
                {
                    cmd.CommandText = "SELECT * FROM USERS WHERE EMAIL=@Username";
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@Username", Email);
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                            used = true;
                    }
                }
            }
            catch (SQLiteException e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                connection.Close();
            }
            return used;
        }

        public User isPasswordRight(string Username,string Password)
        {
            User user = null;
            //string hash = EncryptService.getHashFromString(Password);
            try
            {
                connection.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(connection))
                {
                    cmd.CommandText = "SELECT * FROM USERS WHERE USERNAME=@Username and PASSWORD=@Password";
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@Username", Username);
                    cmd.Parameters.AddWithValue("@Password", Password);
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();
                            user = new User();
                            user.ID = Int32.Parse(reader["ID"].ToString());
                            user.Username = reader["Username"].ToString();
                            user.Password = reader["Password"].ToString();
                            user.Email = reader["Email"].ToString();
                        }
                            
                    }
                }
            }
            catch (SQLiteException e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                connection.Close();
            }
            return user;
        }

        #endregion

        #endregion
    }
}
