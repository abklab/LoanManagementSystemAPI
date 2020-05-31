using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;


namespace Proviso_1_2.Models
{
    public class TransactionServices
    {
        #region Data Access Operations

        string connectionString = ConfigurationManager.ConnectionStrings["PROVISIO_ConnectionString"].ToString();

        SqlConnection connection;

        /// <summary>
        /// user to return  a single value from a stored procedure;
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        string GetSingleValue(SqlCommand cmd)
        {
            string output = "";
            string msg = "";
            try
            {
                connection = new SqlConnection(connectionString);
                using (connection)
                {
                    cmd.Connection = connection;
                    cmd.CommandType = CommandType.StoredProcedure;

                    var parameter = new SqlParameter("@outputValue", SqlDbType.VarChar);
                    parameter.Direction = ParameterDirection.ReturnValue;

                    cmd.Parameters.Add(parameter);
                    connection.Open();
                    cmd.ExecuteNonQuery();
                    output = parameter.Value.ToString();
                }
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            finally
            {
                connection.Close();
                cmd.Dispose();
            }
            return output;
        }

        DataTable GetData(SqlCommand cmd)
        {
            DataTable data = new DataTable();
            string msg = "";
            try
            {
                connection = new SqlConnection(connectionString);
                using (connection)
                {
                    cmd.Connection = connection;
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(data);
                }
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }

            return data;
        }

        string Save(SqlCommand cmd)
        {
            connection = new SqlConnection(connectionString);
            string msg = "";
            try
            {
                using (connection)
                {
                    cmd.Connection = connection;
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (connection.State == ConnectionState.Closed)
                        connection.Open();
                    int i = cmd.ExecuteNonQuery();
                    if (i > 0)
                        msg = "Success";
                    else
                        throw new Exception("Something went wrong.");
                }
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            finally
            {
                connection.Close();
                cmd.Dispose();
            }
            return msg;
        }
        #endregion
        //Get All
        public IEnumerable<Transactions> GetTransactions()
        {
            try
            {
                using (var command = new SqlCommand())
                {
                    var transactionslist = new List<Transactions>();

                    command.CommandText = "[usp_Get_All_Transactions]";

                    var dt = GetData(command);

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        foreach (DataRow r in dt.Rows)
                        {
                            var transaction = new Transactions
                            {
                                EntryID = (int)r["EntryID"],
                                RefNo = r["RefNo"].ToString(),
                                Amount = Convert.ToDecimal(r["Amount"]),
                                B_AccountNumber = r["B_AccountNumber"].ToString(),
                                MomoNumber = r["MomoNumber"].ToString(),
                                MNO = r["MNO"].ToString(),
                                LastUpdated = r["LastUpdated"].ToString()
                            };
                            transactionslist.Add(transaction);
                        }
                    }
                    return transactionslist;
                }
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
            }
            return null;
        }
        //Get Transaction by RefNo
        public BankTransaction GetBankTransactionByRefNo(string ref_number)
        {
            using (var command = new SqlCommand())
            {
                command.CommandText = "usp_Get_Bank_Transaction_RefNo";
                command.Parameters.AddWithValue("@refNo", ref_number);

                var dt = GetData(command);

                if (dt != null && dt.Rows.Count > 0)
                {

                    var r = dt.Rows[0];

                    var transaction = new BankTransaction
                    {
                        EntryID = (int)r["EntryID"],
                        RefNo = r["RefNo"].ToString(),
                        Amount = Convert.ToDecimal(r["Amount"]),
                        B_AccountNumber = r["B_AccountNumber"].ToString(),
                        LastUpdated = r["LastUpdated"].ToString()
                    };
                    return transaction;
                }
                return null;
            }
        }

        public MomoTransaction GetMomoTransactionByRefNo(string ref_number)
        {
            using (var command = new SqlCommand())
            {
                command.CommandText = "usp_Get_Momo_Transaction_RefNo";
                command.Parameters.AddWithValue("@refNo", ref_number);

                var dt = GetData(command);

                if (dt != null && dt.Rows.Count > 0)
                {
                    var r = dt.Rows[0];
                    var transaction = new MomoTransaction
                    {
                        EntryID = (int)r["EntryID"],
                        CUSTOMERREFERENCEID = r["RefNo"].ToString(),
                        AMOUNT = Convert.ToDecimal(r["Amount"]),
                        MSISDN = r["MomoNumber"].ToString(),
                        MNO = r["MNO"].ToString()
                    };
                    return transaction;
                }
                return null;
            }
        }

        //Add or Post Transactions
        public string PostBankTransaction(BankTransaction transaction)
        {
            string msg = "";
            try
            {
                var last_accessed = DateTime.Now;
                using (var command = new SqlCommand())
                {
                    command.CommandText = "[usp_Bank_LoanRepayment]";

                    command.Parameters.AddWithValue("@RefNo ", transaction.RefNo);
                    command.Parameters.AddWithValue("@B_AccountNumber ", transaction.B_AccountNumber);
                    command.Parameters.AddWithValue("@Amount", transaction.Amount);
                    command.Parameters.AddWithValue("@TransactionID", transaction.TransactionID);

                    var result = Save(command);
                    msg = result;
                }
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            return msg;
        }

        //Add or Post Transactions
        public string PostMomoTransaction(MomoTransaction momoTransaction)
        {
            string msg = "";
            try
            {
                var last_accessed = DateTime.Now;
                using (var command = new SqlCommand())
                {
                    command.CommandText = "[usp_Momo_LoanRepayment]";

                    command.Parameters.AddWithValue("@RefNo ", momoTransaction.CUSTOMERREFERENCEID);
                    command.Parameters.AddWithValue("@MomoNumber ", momoTransaction.MSISDN);
                    command.Parameters.AddWithValue("@Amount", momoTransaction.AMOUNT);
                    command.Parameters.AddWithValue("@TransactionID", momoTransaction.TXNID);
                    command.Parameters.AddWithValue("@Type", momoTransaction.TYPE);
                    command.Parameters.AddWithValue("@CompanyName", momoTransaction.COMPANYNAME);

                    command.Parameters.AddWithValue("@ReferenceID", momoTransaction.RefID);
                    command.Parameters.AddWithValue("@MNO ", momoTransaction.MNO);

                    var result = Save(command);

                    msg = result;
                }
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            return msg;
        }

        /// <summary>
        /// Saves Loan application
        /// </summary>
        /// <param name="loanApplication"></param>
        /// <returns></returns>
        public string SaveLoanRequest(LoanApplication loanApplication)
        {
            string msg = "";
            try
            {
                using (var command = new SqlCommand())
                {
                    command.CommandText = "[usp_CreateLoanRequest]";
                    command.Parameters.AddWithValue("@ApplicationID", loanApplication.ApplicationID);
                    command.Parameters.AddWithValue("@FullName", loanApplication.FullName);
                    command.Parameters.AddWithValue("@RequestAmount", loanApplication.RequestAmount);
                    command.Parameters.AddWithValue("@SectorID", loanApplication.SectorID);
                    command.Parameters.AddWithValue("@ContactNumber", loanApplication.ContactNumber);
                    command.Parameters.AddWithValue("@Location", loanApplication.Location);
                    command.Parameters.AddWithValue("@NearestLandmark", loanApplication.NearestLandmark);
                    command.Parameters.AddWithValue("@DistributionMode", loanApplication.DistributionMode);
                    command.Parameters.AddWithValue("@MNO", loanApplication.MNO);
                    command.Parameters.AddWithValue("@MomoNumber", loanApplication.MomoNumber);
                    command.Parameters.AddWithValue("@BankAccountNumber", loanApplication.BankAccountNumber);
                    //command.Parameters.AddWithValue("@RequestDate", loanApplication.RequestDate);
                    command.Parameters.AddWithValue("@Comments", loanApplication.Comments);
                    var result = Save(command);
                    msg = result;
                }
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            return msg;
        }

        public bool ValidateCustomerReferenceID(string referenceID)
        {
            string storedproc = "usp_validateCustomerRefernceID";
            var cmd = new SqlCommand();
            cmd.CommandText = storedproc;
            cmd.Parameters.AddWithValue("@refrenceID", referenceID);
            var count = GetSingleValue(cmd);
            var returnedValue = Convert.ToInt16(count);
            return returnedValue > 0 ? true : false;
        }

        internal void LogTransaction(LoanManagementSystemAPI.Response.COMMAND RESPONSE_COMMAND,decimal? AMOUNT=0)
        {
            string msg = "";
            try
            {

                var last_accessed = DateTime.Now;
                using (var command = new SqlCommand())
                {
                    command.CommandText = "[usp_SaveTransactionLog]";

                    command.Parameters.AddWithValue("@TYPE", RESPONSE_COMMAND.TYPE);
                    command.Parameters.AddWithValue("@TXNID", RESPONSE_COMMAND.TXNID);
                    command.Parameters.AddWithValue("@MSISDN", RESPONSE_COMMAND.MSISDN);
                    command.Parameters.AddWithValue("@RESULT", RESPONSE_COMMAND.RESULT);
                    command.Parameters.AddWithValue("@ERRORCODE", RESPONSE_COMMAND.ERRORCODE);
                    command.Parameters.AddWithValue("@ERRORDESCRIPTION", RESPONSE_COMMAND.ERRORDESCRIPTION);
                    command.Parameters.AddWithValue("@FLAG", RESPONSE_COMMAND.FLAG);
                    command.Parameters.AddWithValue("@CONTENT", RESPONSE_COMMAND.CONTENT);
                    command.Parameters.AddWithValue("@AMOUNT", AMOUNT);

                    var result = Save(command);

                    msg = result;
                }
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }

        }

    }
}
