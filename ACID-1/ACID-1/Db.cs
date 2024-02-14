//#define USE_OCC

using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Numerics;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;


namespace ACID_1
{
    /// <summary>
    /// SQL transaction library. May operate in PCC or OCC as requested
    /// </summary>
    public class Db
    {
        SqlConnection connDB;
        SqlTransaction connTransaction = null;
#if USE_OCC
        private byte[] sqlRowVersion;
        private UInt64 sqlRowVersion64;
#endif
        string connectionString = "Data Source=127.0.0.1;Initial Catalog=xxx;Persist Security Info=True;User ID=sa;Password=@12tf56so;TrustServerCertificate=True";

        public SqlTransaction BeginTransaction()
        {
            connTransaction = connDB.BeginTransaction();
            return connTransaction;
        }

        public SqlTransaction BeginTransaction(System.Data.IsolationLevel theLevel)
        {
            connTransaction = connDB.BeginTransaction(theLevel);
            return connTransaction;
        }

        public void Commit(SqlTransaction theTransaction)
        {
            try
            {
                theTransaction.Commit();
            }
            catch (Exception exC)
            {
                Console.WriteLine("Commit Exception Type: {0}", exC.GetType());
                Console.WriteLine("  Message: {0}", exC.Message);
                try
                {
                    theTransaction.Rollback();
                }
                catch (Exception exR)
                {
                    Console.WriteLine("Rollback Exception Type: {0}", exR.GetType());
                    Console.WriteLine("  Message: {0}", exR.Message);
                }
            }
        }

        public void RollBack(SqlTransaction theTransaction)
        {
            try
            {
                theTransaction.Rollback();
            }
            catch (Exception exR)
            {
                Console.WriteLine("Rollback Exception Type: {0}", exR.GetType());
                Console.WriteLine("  Message: {0}", exR.Message);
            }
        }

        private void DbExec(SqlConnection connDB, string theCommand)
        {
            try
            {
                SqlCommand cmd = new SqlCommand(theCommand, connDB);
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                Console.WriteLine("SQL error: " + ex.ToString());
                Console.WriteLine("- command: " + theCommand);
                Console.ReadLine();
                System.Environment.Exit(1);
            }
        }

        // Convert an object to a byte array
        private static byte[] ObjectToByteArray(Object obj)
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        public int Read()
        {
            SqlCommand cmd = new SqlCommand("Select * FROM Items WHERE itemName='Bad Jokes'", connDB);
            cmd.Transaction = connTransaction;
            try
            {
                SqlDataReader myReader = cmd.ExecuteReader();
                myReader.Read();
                int result = int.Parse(myReader["amount"].ToString());
#if USE_OCC
                sqlRowVersion = myReader["uniq"] as byte[];
                sqlRowVersion64 = 0;
                foreach (var myByte in sqlRowVersion)
                {
                    sqlRowVersion64 <<= 8;
                    sqlRowVersion64 += myByte;
                }
#endif
                myReader.Close();
                return result;
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return 0;
        }

        public bool Write(int newValue)
        {
#if USE_OCC
            SqlConnectionStringBuilder connStringBuilder = new SqlConnectionStringBuilder();
            SqlCommand cmd = new SqlCommand("UPDATE Items SET amount=" + newValue.ToString() + "  WHERE itemName=@itemName AND uniq=" + sqlRowVersion64.ToString(), connDB);
            cmd.Parameters.AddWithValue("@itemName", "Bad Jokes");
#else
            SqlCommand cmd = new SqlCommand("UPDATE Items SET amount=" + newValue.ToString() + "  WHERE itemName='Bad Jokes'", connDB);
#endif
            cmd.Transaction = connTransaction;
            cmd.CommandType = CommandType.Text;
            try
            {
                int rowsAffected = cmd.ExecuteNonQuery();
#if USE_OCC
                if (rowsAffected == 0)
                {
                    // FAILED - ROLLBACK
                    return false;
                }
#endif
                return true;
            }
            catch (SqlException ex)
            {
                // Console.WriteLine(ex.ToString());
                return false;
            }
        }


        public Db(bool doCreate)
        {
            connDB = new SqlConnection(connectionString);
            try
            {
                connDB.Open();

            }
            catch (SqlException ex)
            {
                Console.WriteLine("SQL connect error: " + ex.ToString());
                Console.ReadLine();
                System.Environment.Exit(1);
            }
            Console.WriteLine("Database connection opened");
            if (doCreate)
            {
                try
                {
#if USE_OCC
                    DbExec(connDB, "DROP TABLE IF EXISTS \"Items\"");
                    DbExec(connDB, "CREATE TABLE \"Items\" (\"uniq\" ROWVERSION, \"itemName\" VARCHAR(50) NOT NULL DEFAULT NULL, \"amount\" INT NULL DEFAULT 0, PRIMARY KEY (\"itemName\") )");
                    DbExec(connDB, "INSERT INTO Items (itemName, amount) VALUES ('Bad Jokes', 50)");
#else
                    DbExec(connDB, "DROP TABLE IF EXISTS \"Items\"");
                    DbExec(connDB, "CREATE TABLE \"Items\" (\"itemName\" VARCHAR(50) NOT NULL DEFAULT NULL, \"amount\" INT NULL DEFAULT 0, PRIMARY KEY (\"itemName\") )");
                    DbExec(connDB, "INSERT INTO Items (itemName, amount) VALUES ('Bad Jokes', 50)");
#endif
                }
                catch
                {

                }
            }
        }

        public void Close()
        {
            connDB.Close();
        }
    }
}
