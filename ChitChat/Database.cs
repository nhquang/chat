﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Configuration;
using System.Windows.Forms;

namespace ChitChat
{
    public class Database : IDisposable
    {
        private SqlConnection sql_ { get; set; }
        private SqlCommand sqlCommand_ { get; set; }

        public Database()
        {
            sql_ = new SqlConnection(ConfigurationSettings.AppSettings["connectionString"].Trim().Replace("{your_password}", decryption(ConfigurationSettings.AppSettings["password"].Trim())));
            if (sql_.State == System.Data.ConnectionState.Closed)
                sql_.Open();

        }
        public void addUser(User user)
        {
            var data = new Dictionary<string, object>();
            data.Add("@name_", user.name_);
            data.Add("@username_", user.username_);
            data.Add("@pass_", user.pass_);
            data.Add("@age_", user.age_);
            data.Add("@male_", user.male_);
            data.Add("@note_", user.note_);

            this.constructStoredProcedure("AddUser", data);
            sqlCommand_.ExecuteNonQuery();
        }
        public bool findUser(User user)
        {
            var data = new Dictionary<string, object>();
            data.Add("@Username", user.username_);
            this.constructStoredProcedure("SelectUser", data);
            var rslt = sqlCommand_.ExecuteReader();
            bool hasRows = rslt.HasRows;
            rslt.Close();
            return hasRows;
        }
        private void constructStoredProcedure(string proc, Dictionary<string,object> parameters)
        {
            sqlCommand_ = new SqlCommand(proc, sql_);
            sqlCommand_.CommandType = System.Data.CommandType.StoredProcedure;
            foreach(var item in parameters)
                sqlCommand_.Parameters.Add(new SqlParameter(item.Key, item.Value));
            //sqlCommand_.ExecuteReader();
        }
        private static string encryption(string pwd)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(pwd);

            return Convert.ToBase64String(bytes);
        }
        private static string decryption(string encrypted)
        {
            byte[] bytes = Convert.FromBase64String(encrypted);
            return Encoding.ASCII.GetString(bytes);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.
                if(sql_?.State == System.Data.ConnectionState.Open)
                    sql_.Dispose();
                sqlCommand_?.Dispose();
                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~Database() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            GC.SuppressFinalize(this);
        }
        #endregion

    }
}
