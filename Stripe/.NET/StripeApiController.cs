[Route("api/stripe")]
    [ApiController]
    public class StripeApiController: BaseApiController
    {
        private IStripeService _service = null;
        private IAuthenticationService<int> _authService = null;
        public StripeApiController(IStripeService service, IAuthenticationService<int> authService, ILogger<StripeApiController> logger) : base(logger)
        {
            _service = service;
            _authService = authService;
        }
        [HttpPost("checkout")]
        public ActionResult<ItemResponse<string>> CreateCheckout()
        {
            ObjectResult result = null;

            try
            {
                string sessionId = _service.CreateSession();
                ItemResponse<string> response = new ItemResponse<string>() { Item = sessionId };

                result = Created201(response);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                ErrorResponse response = new ErrorResponse(ex.Message);

                result = StatusCode(500, response);
            }

            return result;
        }

        [HttpPost("subscription")]

        public ActionResult<ItemResponse<string>> CreateSubscription(string priceId, string customerId)
        {
            ObjectResult result = null;

            try
            {
                string sessionId = _service.CreateSubscriptionSession(priceId, customerId);
                ItemResponse<string> response = new ItemResponse<string>() { Item = sessionId };

                result = Created201(response);
            }
            catch(Exception ex)
            {
                Logger.LogError(ex.ToString());
                ErrorResponse response = new ErrorResponse(ex.Message);

                result = StatusCode(500, response);
            }
            return result;
        }

        [HttpGet("subscriptions")]
        public ActionResult<ItemsResponse<StripeProduct>> GetSubscriptions()
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                List<StripeProduct> list = _service.GetSubscriptionsProducts();

                if(list == null)
                {
                    code = 404;
                    response = new ErrorResponse("Application Resource not found");
                }
                else
                {
                    response = new ItemsResponse<StripeProduct> { Items = list };
                }
            }
            catch(Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
                Logger.LogError(ex.ToString());
            }
            return StatusCode(code, response);
        }

        [HttpGet("pagedsubscriptions")]
        public ActionResult<ItemResponse<Paged<Subscription>>> GetPagedSubscriptions(int pageIndex, int pageSize)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                Paged<Subscription> paged = _service.GetSubscriptions(pageIndex, pageSize);

                if(paged == null)
                {
                    code = 404;
                    response = new ErrorResponse("Application Resource not found");
                }
                else
                {
                    response = new ItemResponse<Paged<Subscription>> { Item = paged };
                }
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
                Logger.LogError(ex.ToString());
            }
            return StatusCode(code, response);
        }

        [HttpGet("subscription/createdby")]
        public ActionResult<ItemResponse<CurrentSubscription>> GetSubscriptionByCreatedById(int createdById)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                CurrentSubscription subscription = _service.GetSubscriptionByCreatedById(createdById);

                if (subscription == null)
                {
                    code = 404;
                    response = new ErrorResponse("Application Resourse not found");
                }

                else
                {
                    response = new ItemResponse<CurrentSubscription> { Item = subscription };
                } 
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
                Logger.LogError(ex.ToString());
            }
            return StatusCode(code, response);
        }

        [HttpGet("subscription/update")]
        public ActionResult<ItemResponse<Stripe.Subscription>> UpdateSubscription(string subscriptionId, string priceId)
        {
            int code = 200;
            BaseResponse response = null;
            int userId = _authService.GetCurrentUserId();

            try
            {
                Stripe.Subscription subscription = _service.UpdateStripeSubscription(subscriptionId, priceId);

                if(subscription == null)
                {
                    code = 404;
                    response = new ErrorResponse("Application Resource not found");
                }

                else
                {
                    response = new ItemResponse<Stripe.Subscription> { Item = subscription };
                }
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
                Logger.LogError(ex.ToString());
            }

            return StatusCode(code, response);
        }

        [HttpPost("success")]
        public ActionResult<ItemResponse<int>> AddSubscription(string session_Id)
        {
            ObjectResult result = null;

            try
            {
                int userId = _authService.GetCurrentUserId();
                int id = _service.AddSubscription(session_Id, userId);

                ItemResponse<int> response = new ItemResponse<int>() { Item = id };
                result = Created201(response);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                ErrorResponse response = new ErrorResponse(ex.Message);

                result = StatusCode(500, response);
            }
            return result;
        }

        [HttpPost("checkoutsuccess")]
        public ActionResult<ItemResponse<int>> AddTransaction(string session_Id)
        {
            ObjectResult result = null;

            try
            {
                //int userId = _authService.GetCurrentUserId(); // will change when we are able to have props in React
                int userId = 1;
                int id = _service.AddTrasaction(session_Id, userId);

                ItemResponse<int> response = new ItemResponse<int>() { Item = id };
                result = Created201(response);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                ErrorResponse response = new ErrorResponse(ex.Message);

                result = StatusCode(500, response);
            }
            return result;
        }

        [HttpGet("invoices")]
        public ActionResult<ItemResponse<object>> GetInvoices(string customerId)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                object invoices = _service.GetInvoice(customerId);

                if (invoices == null)
                {
                    code = 404;
                    response = new ErrorResponse("App Resource not found");
                }
                else
                {
                    response = new ItemResponse<object>() { Item = invoices };
                }
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
                Logger.LogError(ex.ToString());
            }
            return StatusCode(code, response);
        }

        [HttpGet("transactions")]
        public ActionResult<ItemResponse<Paged<Transaction>>> GetTransactionsByUserId(int pageIndex, int pageSize, int userId)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                Paged<Transaction> paged = _service.GetTransactionsByUserId(pageIndex, pageSize, userId);

                if (paged == null)
                {
                    code = 404;
                    response = new ErrorResponse("Application Resource not found");
                }
                else
                {
                    response = new ItemResponse<Paged<Transaction>> { Item = paged };
                }
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
                Logger.LogError(ex.ToString());
            }
            return StatusCode(code, response);
        }

        [HttpGet("transactions/{id:int}")]
        public ActionResult<ItemResponse<Transaction>> GetTransactionById(int id)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                Transaction transaction = _service.GetTransactionById(id);

                if (transaction == null)
                {
                    code = 404;
                    response = new ErrorResponse("Application Resource not found");
                }
                else
                {
                    response = new ItemResponse<Transaction> { Item = transaction };
                }
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
                Logger.LogError(ex.ToString());
            }
            return StatusCode(code, response);
        }
        [HttpPost("transactions")]
        public ActionResult<ItemResponse<int>> CreateTransaction(TransactionAddRequest model)
        {
            ObjectResult result = null;
            try
            {
                int userId = 1;
                //int userId = _authService.GetCurrentUserId(); // will change when we are able to have props in React
                int id = _service.CreateTransaction(model, userId);
                ItemResponse<int> response = new ItemResponse<int>() { Item = id };

                result = Created201(response);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                ErrorResponse response = new ErrorResponse(ex.Message);
                result = StatusCode(500, response);
            }
            return result;
        }

        [HttpPost("paymentaccounts")]
        public ActionResult<ItemResponse<int>> CreatePaymentAccount(PaymentAccountAddRequest model)
        {
            ObjectResult result = null;
            try
            {
                int userId = _authService.GetCurrentUserId();
                int id = _service.CreatePaymentAccount(model, userId);
                ItemResponse<int> response = new ItemResponse<int>() { Item = id };

                result = Created201(response);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                ErrorResponse response = new ErrorResponse(ex.Message);
                result = StatusCode(500, response);
            }
            return result;
        }

        [HttpPut("paymentaccounts/{id:int}")]
        public ActionResult<SuccessResponse> UpdatePaymentAccount(PaymentAccountUpdateRequest model)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                int userId = _authService.GetCurrentUserId();
                _service.UpdatePaymentAccount(model, userId);
                response = new SuccessResponse();
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
                Logger.LogError(ex.ToString());
            }
            return StatusCode(code, response);
        }

        [HttpGet("paymentaccounts")]
        public ActionResult<ItemResponse<Paged<PaymentAccount>>> GetPaymentAccounts(int pageIndex, int pageSize)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                Paged<PaymentAccount> paged = _service.GetPaymentAccounts(pageIndex, pageSize);

                if (paged == null)
                {
                    code = 404;
                    response = new ErrorResponse("Application Resource not found");
                }
                else
                {
                    response = new ItemResponse<Paged<PaymentAccount>> { Item = paged };
                }
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
                Logger.LogError(ex.ToString());
            }
            return StatusCode(code, response);
        }
        [HttpGet("paymentaccountscreatedby")]
        public ActionResult<ItemResponse<Paged<PaymentAccount>>> GetPaymentAccountsByCreatedBy(int pageIndex, int pageSize, int createdBy)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                Paged<PaymentAccount> paged = _service.GetPaymentAccountsByCreatedBy(pageIndex, pageSize, createdBy);

                if (paged == null)
                {
                    code = 404;
                    response = new ErrorResponse("Application Resource not found");
                }
                else
                {
                    response = new ItemResponse<Paged<PaymentAccount>> { Item = paged };
                }
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
                Logger.LogError(ex.ToString());
            }
            return StatusCode(code, response);
        }

        [HttpGet("paymentaccounts/{id:int}")]
        public ActionResult<ItemResponse<PaymentAccount>> GetPaymentAccountById(int id)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                PaymentAccount paymentAccount = _service.GetPaymentAccountById(id);

                if (paymentAccount == null)
                {
                    code = 404;
                    response = new ErrorResponse("Application Resource not found");
                }
                else
                {
                    response = new ItemResponse<PaymentAccount> { Item = paymentAccount };
                }
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
                Logger.LogError(ex.ToString());
            }
            return StatusCode(code, response);
        }

        [HttpDelete("paymentaccounts/{id:int}")]
        public ActionResult<SuccessResponse> DeletePaymentAccount(int id)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                _service.DeletePaymentAccount(id);
                response = new SuccessResponse();
            } 
            catch(Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
            }
            return StatusCode(code, response);
        }
   }
