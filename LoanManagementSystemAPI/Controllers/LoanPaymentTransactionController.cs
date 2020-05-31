using LoanManagementSystemAPI.Models;
using Proviso_1_2.Models;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace Proviso_1_2.Controllers
{
    [RoutePrefix("provisio/api/v1000/_transactions")]
    public class LoanPaymentTransactionController : ApiController
    {
        public LoanPaymentTransactionController()
        {
        }

        TransactionServices services = new TransactionServices();


        /// <summary>
        /// Get Bank repayment By RefNo
        /// </summary>
        /// <param name="ref_number"></param>
        /// <returns>Momo repayment Account</returns>       
        [ResponseType(typeof(MomoTransaction))]
        [Route("maccount/{ref_number}/mp-500")]
        public IHttpActionResult GetByMomoNumber(string ref_number)
        {
            var transaction = services.GetMomoTransactionByRefNo(ref_number);
            if (transaction == null)
                return StatusCode(HttpStatusCode.NotFound);
            return Ok(transaction);
        }

        /// <summary>
        /// Get Bank Repayment By RefNo
        /// </summary>
        /// <param name="ref_number"></param>
        /// <returns>Bank Account</returns>   
        [ResponseType(typeof(BankTransaction))]
        [Route("baccount/{ref_number}/bp-100")]
        public IHttpActionResult GetByBankAccount(string ref_number)
        {
            var transaction = services.GetBankTransactionByRefNo(ref_number);
            if (transaction == null)
                return StatusCode(HttpStatusCode.NotFound);
            return Ok(transaction);
        }


        // POST: api/LoanPaymentTransaction
        [Route("_bankpost/bp-100/repayments")]
        public IHttpActionResult PostBankAccount([FromBody]BankTransaction banktransaction)
        {
            if (string.IsNullOrWhiteSpace(banktransaction.B_AccountNumber))
                return StatusCode(HttpStatusCode.BadRequest);
            else
            {
                var result = services.PostBankTransaction(banktransaction);

                string responseTime = DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss tt");

                return result == "Success" ? Ok($"S-100-B {responseTime}") : Ok($"R-500-B {responseTime}");
            }
        }


        // POST: api/LoanPaymentTransaction
        [Route("_momopost/mp-500/repayments")]
        public IHttpActionResult PostByMomoNumber([FromBody]COMMAND command)
        {
            if (command == null)
                BadRequest("Wrong Input body. Please check and try again");

            var momotransaction = new MomoTransaction
            {
                 AMOUNT=command.AMOUNT,
                 TYPE=command.TYPE,
                 TXNID=command.TXNID,
                 MSISDN=command.MSISDN,
                 COMPANYNAME=command.COMPANYNAME,
                 CUSTOMERREFERENCEID=command.CUSTOMERREFERENCEID               
            };

            string responseTime = DateTime.UtcNow.ToString("dd-MM-yyyy h:mm:ss tt");
            var response = new LoanManagementSystemAPI.Response.COMMAND
            {
                TYPE = momotransaction.TYPE,
                TXNID = momotransaction.TXNID,
                RefID = momotransaction.RefID,
                MSISDN = momotransaction.MSISDN,
            };

            var isValidRef = services.ValidateCustomerReferenceID(momotransaction.CUSTOMERREFERENCEID);
            if (isValidRef)
            {
                var result = services.PostMomoTransaction(momotransaction);

                if (result == "Success")
                {
                    response.RESULT = "TS";
                    response.ERRORCODE = "error000";
                    response.ERRORDESCRIPTION = "Successful transaction";
                    response.FLAG = "Y";
                    response.CONTENT = $"Transaction (TXNID: {momotransaction.TXNID}) submitted successfully ({responseTime}).";
                    services.LogTransaction(response,momotransaction.AMOUNT);//this logs transaxtions
                    return Ok(response);
                }
                else
                {
                    response.RESULT = "TF";
                    response.ERRORCODE = "error100";
                    response.ERRORDESCRIPTION = "General Error";
                    response.FLAG = "N";
                    response.CONTENT = $"Transaction (TXNID: {momotransaction.TXNID}) has failed ({responseTime}).";
                    services.LogTransaction(response, momotransaction.AMOUNT);
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.BadRequest, response));
                }
            }
            else
            {
                response.RESULT = "TF";
                response.ERRORCODE = "error010";
                response.ERRORDESCRIPTION = "Invalid Customer Reference Number";
                response.FLAG = "N";
                response.CONTENT = $"Transaction (TXNID: {momotransaction.TXNID}) has failed ({responseTime}).";
                services.LogTransaction(response, momotransaction.AMOUNT);
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.BadRequest, response));
            }
        }


        // POST: api/LoanRequestTransaction
        [HttpPost]
        [Route("_services/2020/loanrequest")]
        public IHttpActionResult PostLoanRequests([FromBody]LoanApplication loanApplication)
        {
            // if(!ModelState.IsValid)
            if (string.IsNullOrWhiteSpace(loanApplication.FullName))
                return StatusCode(HttpStatusCode.BadRequest);
            else
            {
                var result = services.SaveLoanRequest(loanApplication);

                string responseTime = DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss tt");

                return result == "Success" ? Ok($"S-100-M {responseTime}") : Ok($"R-500-M {responseTime}. Something went wrong.");
            }
        }


    }
}
